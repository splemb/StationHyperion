using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    //Referenced Components
    [Header("Referenced Components")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] Transform grapplePoint;
    [SerializeField] Transform beamEmitPoint;
    [SerializeField] Rigidbody swingFollow;
    [SerializeField] GameObject shot;
    [SerializeField] Transform shotOrigin;
    [SerializeField] AudioSource shotSfx;
    Rigidbody rb;
    CapsuleCollider standCollider;
    SphereCollider crouchCollider;
    Cinemachine.CinemachineVirtualCamera vCam;
    public SaveData saveData;

    //Input Variables
    Vector2 moveAxis;

    //Movement Variables
    Vector2 hVelocity;
    float vVelocity;
    bool isCrouching;
    bool isSprinting;
    bool isJumping;
    bool tryJump;
    bool stompFlag;
    float currentGravityScale;
    int dashCounter = 3;

    //Player State
    enum PlayerState { Standard, Swinging, Flinging, Sliding, Crouching, Dead }
    [SerializeField] PlayerState state;

    //Player Parameters
    [Header("Player Parameters")]
    [SerializeField] float gravityScale = 3;
    [SerializeField] float speed = 120;
    [SerializeField] float crouchSpeedMultiplier = 0.5f;
    [SerializeField] float sprintSpeedMultiplier = 1.5f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float jumpGravityScaleModifier = 0.5f;
    [SerializeField] public LayerMask environmentMask;
    [SerializeField] LayerMask aimMask;

    //Health
    [Header("Player Health")]
    public float maxHealth = 100;
    public float health;
    [SerializeField] float invincibilityTimer;

    //Camera
    [Header("Camera")]
    [SerializeField] float minAngle = -80f;
    [SerializeField] float maxAngle = 80f;
    [SerializeField] float sensitivityY = 3.0f;
    [SerializeField] float sensitivityX = 3.0f;

    float camY = 0.0f;
    float camX = 0.0f;
    float camDeltaX = 0.0f;
    float camDeltaY = 0.0f;

    //SFX
    [Header("SFX")]
    [SerializeField] AudioSource walkAudio;
    
    [SerializeField] AudioClip[] footsteps;
    [SerializeField] float stepSize = 1f;
    float nextFootstep = 0f;

    [SerializeField] AudioSource miscAudio;
    [SerializeField] AudioClip[] sounds;

    [SerializeField] AudioSource slideAudio;

    void Start()
    {
        //Get Components
        rb = GetComponent<Rigidbody>();
        standCollider = GetComponent<CapsuleCollider>();
        crouchCollider = GetComponent<SphereCollider>();
        vCam = GameObject.Find("Player vCam").GetComponent<Cinemachine.CinemachineVirtualCamera>();

        Init();
        
    }

    void Init()
    {
        //Init Variables
        currentGravityScale = gravityScale;
        ChangeState(PlayerState.Standard);
        LoadData();
        health = maxHealth;

        //Init Camera
        cameraTransform.parent = null;
        camX = transform.rotation.eulerAngles.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        if (!PauseManager.GAME_IS_PAUSED)
        {
            float fov = vCam.m_Lens.FieldOfView;

            switch (state)
            {
                case PlayerState.Standard:
                    StandardPhysics();
                    if (footsteps.Length > 0) WalkSFX();
                    fov = Mathf.Lerp(fov, 50, Time.deltaTime * 30f);
                    vCam.m_Lens.FieldOfView = fov;
                    break;
                case PlayerState.Swinging:
                    SwingingPhysics();
                    fov = Mathf.Lerp(fov, 50 + Mathf.Clamp(swingFollow.velocity.magnitude, 0f, 50f), Time.deltaTime * 30f);
                    vCam.m_Lens.FieldOfView = fov;
                    break;
                case PlayerState.Flinging:
                    FlingingPhysics();
                    fov = Mathf.Lerp(fov, 50 + Mathf.Clamp(swingFollow.velocity.magnitude, 0f, 50f), Time.deltaTime * 30f);
                    vCam.m_Lens.FieldOfView = fov;
                    break;
                case PlayerState.Sliding:
                    SlidingPhysics();
                    break;
            }

            if (invincibilityTimer > 0) invincibilityTimer -= Time.fixedDeltaTime;
            else invincibilityTimer = 0;
        }
        
    }

    private void LateUpdate()
    {
        if (!PauseManager.GAME_IS_PAUSED)
        {
            Look();
        }
    }

    void StandardPhysics()
    {

        //Horizontal Velocity
        hVelocity = Vector2.ClampMagnitude(new Vector2(moveAxis.x * speed, moveAxis.y * speed), speed) * 2f * Time.fixedDeltaTime;

        //Manual Gravity
        vVelocity = rb.velocity.y;

        if ((CheckGrounded()) && !isJumping)
        {
            
            Mathf.Clamp(vVelocity, 0f, Mathf.Infinity);
            slideAudio.Stop();
            if (tryJump)
            {
                tryJump = false;
                isJumping = true;
                vVelocity = jumpForce;
                walkAudio.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);
            } else
            {
                if (moveAxis != Vector2.zero && vVelocity > 0) SetToGround();
                vVelocity = 0;
            }
        }
        else if (CheckFacingWall(0.1f))
        {

            if (tryJump && saveData.WallJump)
            {
                ChangeState(PlayerState.Flinging);
                walkAudio.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);
                rb.velocity = Vector3.zero;
                rb.AddForce(new Vector3(-transform.forward.x * 40f, jumpForce * 3f, -transform.forward.z * 40f) * 2f, ForceMode.Impulse);
                slideAudio.Stop();
            } else
            {
                if (vVelocity > 0) { 
                    vVelocity += currentGravityScale * Physics.gravity.y * Time.deltaTime;
                    slideAudio.Stop();
                }
                else { 
                    vVelocity += currentGravityScale * Physics.gravity.y * Time.deltaTime * 0.5f;
                    if (!slideAudio.isPlaying) slideAudio.Play();
                    
                }
            }
        }
        else
        {
            slideAudio.Stop();
            vVelocity += currentGravityScale * Physics.gravity.y * Time.deltaTime;
        }

        //Apply velocity to rb
        transform.Rotate(new Vector3(0f, cameraTransform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y, 0f));
        rb.velocity = transform.TransformDirection(new Vector3(hVelocity.x, vVelocity, hVelocity.y));
    }

    void SwingingPhysics()
    {
        transform.position = swingFollow.position;
        transform.Rotate(new Vector3(0f, cameraTransform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y, 0f));
        //beamEmitPoint.GetComponent<ParticleSystem>().transform.LookAt(grapplePoint);
        swingFollow.AddForce(transform.TransformDirection(new Vector3(moveAxis.x, 0f, moveAxis.y)) * speed * 0.01f * Time.fixedDeltaTime, ForceMode.Impulse);
        beamEmitPoint.LookAt(grapplePoint);
    }
    void FlingingPhysics()
    {
        transform.Rotate(new Vector3(0f, cameraTransform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y, 0f));

        if (CheckFacingWall(0.1f))
        {

            if (tryJump)
            {
                slideAudio.Stop();
                rb.AddForce(new Vector3(-transform.forward.x * 40f, jumpForce * 3f, -transform.forward.z * 40f) * 2f, ForceMode.Impulse);
            }
            else
            {
                if (!slideAudio.isPlaying) slideAudio.Play();
                rb.AddForce(transform.TransformDirection(new Vector3(moveAxis.x, 0f, moveAxis.y)) * speed * 0.5f * Time.fixedDeltaTime, ForceMode.Impulse);
            }
        }
        else
        {
            slideAudio.Stop();
            rb.AddForce(transform.TransformDirection(new Vector3(moveAxis.x, 0f, moveAxis.y)) * speed * 0.5f * Time.fixedDeltaTime, ForceMode.Impulse);
        }

        if (CheckGrounded(0.1f))
        {
            slideAudio.Stop();
            ChangeState(PlayerState.Standard);
        }
    }

    void SlidingPhysics()
    {
        hVelocity = Vector2.ClampMagnitude(new Vector2(0, speed), speed) * 3f * Time.fixedDeltaTime;

        vVelocity = rb.velocity.y;

        if (CheckGrounded()) SetToGround();
        else if (CheckFacingWall(0.1f)) ChangeState(PlayerState.Standard);
        else vVelocity += currentGravityScale * Physics.gravity.y * Time.deltaTime;

        //Apply velocity to rb
        transform.Rotate(new Vector3(0f, cameraTransform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y, 0f));
        rb.velocity = transform.TransformDirection(new Vector3(hVelocity.x, vVelocity, hVelocity.y));
    }

    void Look()
    {
        if (state == PlayerState.Dead) return;
        camY += camDeltaY * sensitivityY * Time.deltaTime * 40;
        camX += camDeltaX * sensitivityX * Time.deltaTime * 40;
        camY = Mathf.Clamp(camY, minAngle, maxAngle);

        if (isCrouching) cameraTransform.position = new Vector3(transform.position.x, transform.position.y + (0.8f), transform.position.z);
        else cameraTransform.position = new Vector3(transform.position.x, transform.position.y + (1.8f), transform.position.z);

        cameraTransform.rotation = Quaternion.Euler(new Vector3(camY, camX, 0));
    }

    void WalkSFX()
    {
        if (hVelocity.magnitude > 0 && CheckGrounded(0.5f) && Time.time >= nextFootstep)
        {
            walkAudio.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);
            nextFootstep = Time.time + stepSize / rb.velocity.magnitude;
        }
    }

    public void MouseLook(InputAction.CallbackContext input)
    {
        camDeltaY = input.ReadValue<Vector2>().y;
        camDeltaX = input.ReadValue<Vector2>().x;
    }

    public void Movement(InputAction.CallbackContext input)
    {
        moveAxis = input.ReadValue<Vector2>();
    }

    public void TryJump(InputAction.CallbackContext input)
    {
        if (PauseManager.GAME_IS_PAUSED) return;
        if (state == PlayerState.Dead) return;

        if (input.phase == InputActionPhase.Started)
        {
            if (CheckGrounded(0.5f) && !isJumping) tryJump = true;
            else if (CheckFacingWall(0.1f) && !isJumping) { tryJump = true; rb.velocity = Vector3.zero; }
        }
        
        if (input.phase == InputActionPhase.Canceled)
        {
            Debug.Log("Jump Cancelled");
            isJumping = false;
            tryJump = false;
            if (rb.velocity.y > 0) rb.velocity += new Vector3(0, -rb.velocity.y);
            currentGravityScale = gravityScale;
        }
        
    }

    public void GrappleFire(InputAction.CallbackContext input)
    {
        if (PauseManager.GAME_IS_PAUSED) return;
        if (state == PlayerState.Dead) return;

        if (!saveData.GrapplingHook) return;

        if (input.phase == InputActionPhase.Started)
        {
            //Debug.Log("Click!");
            RaycastHit hit;

            if (!Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 20f, environmentMask))
                Physics.Raycast(cameraTransform.position, transform.forward * 0.4f + Vector3.up, out hit, 20f, environmentMask);

            if (hit.collider != null)
            {
                if (hit.collider.tag == "CantGrapple") return;
                grapplePoint.position = hit.point;
                
                swingFollow.gameObject.SetActive(true);
                beamEmitPoint.gameObject.SetActive(true);

                swingFollow.velocity = rb.velocity;
                swingFollow.position = transform.position;
                float distance = Vector3.Distance(swingFollow.position, grapplePoint.position);
                swingFollow.GetComponent<SpringJoint>().maxDistance = distance * 0.7f;
                ChangeState(PlayerState.Swinging);
            }

            
        }

        else if (input.phase == InputActionPhase.Canceled)
        {
            //Debug.Log("Unclick!");
            if (state == PlayerState.Swinging)
                ChangeState(PlayerState.Flinging);
            swingFollow.gameObject.SetActive(false);
            beamEmitPoint.gameObject.SetActive(false);
        }
    }

    public void ShotFire(InputAction.CallbackContext input)
    {
        if (PauseManager.GAME_IS_PAUSED) return;
        if (state == PlayerState.Dead) return;

        if (input.phase == InputActionPhase.Canceled)
        {
            shotSfx.Play();
            RaycastHit hit;
            Vector3 point;

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 30f, aimMask))
            {
                point = hit.point;

            } else
            {
                point = cameraTransform.position + cameraTransform.forward * 30f;
            }

            Quaternion rotation = Quaternion.LookRotation(point - shotOrigin.position);

            GameObject newShot = Instantiate(shot, shotOrigin.position, rotation);

            newShot.GetComponent<Shot>().SetDamage(1f);
            
        }
    }

    public void TryDash(InputAction.CallbackContext input)
    {
        if (PauseManager.GAME_IS_PAUSED) return;
        if (state == PlayerState.Dead) return;

        if (!saveData.Dash) return;

        if (input.phase == InputActionPhase.Started)
        {
            if (dashCounter > 0)
            {
                switch (state)
                {
                    case PlayerState.Standard:
                        if (!CheckGrounded())
                        {
                            ChangeState(PlayerState.Flinging);
                            rb.velocity = Vector3.zero;
                            rb.AddForce(transform.forward * 100f, ForceMode.Impulse);
                            miscAudio.PlayOneShot(sounds[0]);
                        }
                        break;
                    case PlayerState.Swinging:
                        swingFollow.velocity = Vector3.zero;
                        swingFollow.AddForce(cameraTransform.forward * 50f, ForceMode.Impulse);
                        miscAudio.PlayOneShot(sounds[0]);
                        break;
                    case PlayerState.Flinging:
                        //rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                        rb.velocity = Vector3.zero;
                        rb.AddForce(transform.forward * 100f, ForceMode.Impulse);
                        miscAudio.PlayOneShot(sounds[0]);
                        stompFlag = false;
                        break;
                }
                dashCounter--;
            }
        }
    }

    public void TrySlide(InputAction.CallbackContext input)
    {
        if (PauseManager.GAME_IS_PAUSED) return;
        if (state == PlayerState.Dead) return;

        if (input.phase == InputActionPhase.Started)
        {
            switch (state)
            {
                case PlayerState.Standard:
                    if (CheckGrounded()) 
                    { 
                        if (saveData.Slide) ChangeState(PlayerState.Sliding); 
                    }
                    else
                    {
                        if (saveData.Stomp)
                        {
                            stompFlag = true;
                            ChangeState(PlayerState.Flinging);
                            rb.velocity = Vector3.zero;
                            rb.AddForce(-transform.up * 300f, ForceMode.Impulse);
                            miscAudio.PlayOneShot(sounds[1]);
                        }
                    }
                    break;
                case PlayerState.Flinging:
                    if (saveData.Stomp)
                    {
                        stompFlag = true;
                        rb.velocity = Vector3.zero;
                        rb.AddForce(-transform.up * 300f, ForceMode.Impulse);
                        miscAudio.PlayOneShot(sounds[1]);
                    }
                    break;
            }
        }
        else if (input.phase == InputActionPhase.Canceled)
        {
            if (state == PlayerState.Sliding) ChangeState(PlayerState.Flinging);
        }
    }

    public void TakeDamage(float amt)
    {
        if (invincibilityTimer > 0 || state == PlayerState.Dead) return;
        health -= amt;
        miscAudio.PlayOneShot(sounds[3]);
        invincibilityTimer = 1f;
        if (health <= 0) ChangeState(PlayerState.Dead);
    }

    public void Pause(InputAction.CallbackContext input)
    {
        PauseManager.Pause();
    }

    void ChangeState(PlayerState newState)
    {
        var transposer = vCam.GetCinemachineComponent<Cinemachine.CinemachineTransposer>();

        if (newState == state) return;

        //Leaving State
        switch (state)
        {
            case PlayerState.Standard:
                
                break;
            case PlayerState.Swinging:
                
                break;
            case PlayerState.Flinging:
                if (newState == PlayerState.Standard && stompFlag)
                {
                    stompFlag = false;
                    miscAudio.PlayOneShot(sounds[2]);
                }
                dashCounter = 3;

                break;
            case PlayerState.Sliding:
                transposer.m_FollowOffset = Vector3.zero;
                crouchCollider.enabled = false;
                standCollider.enabled = true;
                slideAudio.Stop();
                break;
            case PlayerState.Dead:
                crouchCollider.enabled = false;
                standCollider.enabled = true;
                transposer.m_FollowOffset = Vector3.zero;
                break;
        }

        state = newState;
        

        //Entering State
        switch (state)
        {
            case PlayerState.Standard:
                Debug.Log("Switched to Standard Physics");
                rb.useGravity = false;
                rb.isKinematic = false;
                nextFootstep = Time.time;
                break;
            case PlayerState.Swinging:
                Debug.Log("Switched to Swinging Physics");
                rb.isKinematic = true;
                break;
            case PlayerState.Flinging:
                Debug.Log("Switched to Flinging Physics");
                rb.velocity = swingFollow.velocity;
                rb.useGravity = true;
                rb.isKinematic = false;
                break;
            case PlayerState.Sliding:
                Debug.Log("Switched to Sliding Physics");
                crouchCollider.enabled = true;
                standCollider.enabled = false;
                transposer.m_FollowOffset = -Vector3.up;
                slideAudio.Play();
                break;
            case PlayerState.Dead:
                transposer.m_FollowOffset = -Vector3.up;
                crouchCollider.enabled = true;
                standCollider.enabled = false;
                rb.useGravity = true;
                rb.isKinematic = false;
                swingFollow.gameObject.SetActive(false);
                beamEmitPoint.gameObject.SetActive(false);
                stompFlag = true;
                slideAudio.Stop();
                StartCoroutine(Death());
                break;
        } 
    }

    bool CheckGrounded(float reach = 0.001f)
    {
        return (Physics.BoxCast(transform.position + (transform.up * 0.5f), new Vector3(0.25f, 0.1f, 0.25f), -transform.up, Quaternion.identity, (0.5f + reach), environmentMask));
    }

    bool CheckFacingWall(float reach= 0.001f)
    {
        return (Physics.BoxCast(transform.position + (transform.up * 0.5f), new Vector3(0.25f, 0.1f, 0.25f), transform.forward, Quaternion.identity, (0.1f + reach), environmentMask));
    }

    void SetToGround(float reach = 0.05f)
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + (transform.up * 0.5f), -transform.up, out hit, .5f + reach)) {
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }
    }

    void LoadData()
    {
        saveData = SerializationManager.Load("file1");

        if (saveData == null) saveData = new SaveData();

        SaveInteraction respawn = GameObject.Find(saveData.respawnPoint).GetComponent<SaveInteraction>();

        if (respawn != null)
        {
            transform.position = respawn.respawnPoint.position;
            transform.rotation = respawn.respawnPoint.rotation;

            respawn.room.SetActive(true);
            
        }
    }

    IEnumerator Death()
    {
        GameObject.Find("FadeCanvas").GetComponent<Animator>().SetTrigger("FadeOut");
        yield return new WaitForSecondsRealtime(1f);
        Init();
        GameObject.Find("FadeCanvas").GetComponent<Animator>().SetTrigger("FadeIn");
    }
}
