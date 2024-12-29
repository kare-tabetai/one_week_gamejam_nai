using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class ScoreManager : MonoSingleton<ScoreManager>
{
    float start_time;
    public int kill_count;
    public void AddKillScore()
    {
        kill_count++;
    }

    public float GetTime() { return start_time - Time.time; }

    private void Start()
    {
        start_time = Time.time;
    }


}
