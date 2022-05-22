using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PauseManager
{
    public static bool GAME_IS_PAUSED = false;

    public static void Pause()
    {
        GAME_IS_PAUSED = !GAME_IS_PAUSED;
        if (GAME_IS_PAUSED)
        {
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }
        else 
        { 
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }
    }
}
