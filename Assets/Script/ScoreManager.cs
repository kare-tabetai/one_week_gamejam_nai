using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class ScoreManager : MonoSingleton<ScoreManager>
{
    public float game_timer;
    public int kill_count;
    public void AddKillScore()
    {
        kill_count++;
    }
}
