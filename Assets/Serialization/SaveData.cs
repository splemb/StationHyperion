using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    private static SaveData _current;

    public static SaveData Current
    {
        get 
        { 
            if (_current == null)
            {
                _current = new SaveData();
            }
            return _current; 
        }
    }

    public bool GrapplingHook;
    public bool Slide;
    public bool Dash;
    public bool Stomp;
    public bool WallJump;

    public string respawnPoint;
}
