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
    [SerializeField] Transform swingFollow;
    [SerializeField] GameObject shot;
    [SerializeField] Transform shotOrigin;
    [SerializeField] AudioSource shotSfx;
    Rigidbody rb;
    CapsuleCollider standCollider;
    SphereCollider crouchCollider;

    //Input Variables
    Vector2 moveAxis;

    //Movement Variables
    Vector2 hVelocity;
    float vVelocity;
    bool isCrouching;
    bool isSprinting;
    bool isJumping;
    bool tryJump;
    float currentGravityScale;

    //Player State
    enum PlayerState { Standard, Swinging, Flinging }
    [SerializeField] PlayerState state;

    //Player Parameters
    [Header("Player Parameters")]
    [SerializeField] float gravityScale = 3;
    [SerializeField] float speed = 120;
    [SerializeField] float crouchSpeedMultiplier = 0.5f;
    [SerializeField] float sprintSpeedMultiplier = 1.5f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float jumpGravityScaleModifier = 0.5f;
    [SerializeField] LayerMask environmentMask;
    [SerializeField] LayerMask aimMask;

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

    void Start()
    {
        //Get Components
        rb = GetComponent<Rigidbody>();
        standCollider = GetComponent<CapsuleCollider>();
        crouchCollider = GetComponent<SphereCollider>();

        //Init Variables
        currentGravityScale = gravityScale;
        ChangeState(PlayerState.Standard);

        //Init Camera
        cameraTransform.parent = null;
        camX = transform.rotation.eulerAngles.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case PlayerState.Standard:
                StandardPhysics();
                if (footsteps.Length > 0) WalkSFX();
                break;
            case PlayerState.Swinging:
                SwingingPhysics();
                break;
            case PlayerState.Flinging:
                FlingingPhysics();
                break;
        }
        
    }

    private void LateUpdate()
    {
        Look();
    }

    void StandardPhysics()
    {

        //Horizontal Velocity
        hVelocity = Vector2.ClampMagnitude(new Vector2(moveAxis.x * speed, moveAxis.y * speed), speed) * 2f * Time.fixedDeltaTime;

        //Manual Gravity
        vVelocity = rb.velocity.y;

        if (CheckGrounded() && !isJumping)
        {
            
            Mathf.Clamp(vVelocity, 0f, Mathf.Infinity);

            if (tryJump)
            {
                tryJump = false;
                isJumping = true;
                vVelocity = jumpForce;
                currentGravityScale = gravityScale * jumpGravityScaleModifier;
            } else
            {
                if (moveAxis != Vector2.zero && vVelocity > 0) SetToGround();
                vVelocity = 0;
            }
        }
        else
        {
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
        swingFollow.GetComponentInChildren<ParticleSystem>().transform.LookAt(grapplePoint);
        swingFollow.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(new Vector3(moveAxis.x, 0f, moveAxis.y)) * speed * 0.01f * Time.fixedDeltaTime, ForceMode.Impulse);
        //swingFollow.LookAt(grapplePoint);
    }
    void FlingingPhysics()
    {
        transform.Rotate(new Vector3(0f, cameraTransform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y, 0f));
        rb.AddForce(transform.TransformDirection(new Vector3(moveAxis.x, 0f, moveAxis.y)) * speed * 0.5f * Time.fixedDeltaTime, ForceMode.Impulse);

        if (CheckGrounded(1f))
        {
            ChangeState(PlayerState.Standard);
        }
    }

    void Look()
    {
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
        if (input.phase == InputActionPhase.Started)
        {
            if (CheckGrounded(0.5f) && !isJumping) tryJump = true;
        }
        
        if (input.phase == InputActionPhase.Canceled)
        {
            isJumping = false;
            tryJump = false;
            currentGravityScale = gravityScale;
        }
        
    }

    public void GrappleFire(InputAction.CallbackContext input)
    {
        if (input.phase == InputActionPhase.Started)
        {
            //Debug.Log("Click!");
            RaycastHit hit;

            if (!Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 20f, environmentMask))
                Physics.Raycast(cameraTransform.position, transform.forward * 0.4f + Vector3.up, out hit, 20f, environmentMask);

            if (hit.collider != null)
            {
                grapplePoint.position = hit.point;
                
                swingFollow.gameObject.SetActive(true);

                swingFollow.GetComponent<Rigidbody>().velocity = rb.velocity;
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
        }
    }

    public void ShotFire(InputAction.CallbackContext input)
    {
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
            
            if (state == PlayerState.Swinging)
                newShot.GetComponent<Shot>().speed += swingFollow.GetComponent<Rigidbody>().velocity.magnitude * Vector3.Dot(point - shotOrigin.position, rb.velocity);
            else
                newShot.GetComponent<Shot>().speed += rb.velocity.magnitude * Vector3.Dot(point - shotOrigin.position, rb.velocity) * Time.deltaTime;
            
        }
    }

    void ChangeState(PlayerState newState)
    {

        if (newState == state) return;

        //Leaving State
        switch (state)
        {
            case PlayerState.Standard:
                
                break;
            case PlayerState.Swinging:
                
                break;
            case PlayerState.Flinging:
                
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
                rb.velocity = swingFollow.GetComponent<Rigidbody>().velocity;
                rb.useGravity = true;
                rb.isKinematic = false;
                break;
        }

        
    }

    bool CheckGrounded(float reach = 0.001f)
    {
        return (Physics.BoxCast(transform.position + (transform.up * 0.5f), new Vector3(0.25f, 0.1f, 0.25f), -transform.up, Quaternion.identity, (0.5f + reach), environmentMask));
    }

    void SetToGround(float reach = 0.02f)
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + (transform.up * 0.5f), -transform.up, out hit, .5f + reach)) {
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }
    }
}
