using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainCameraController : MonoSingleton<MainCameraController>
{
    public UnityEvent on_blended_ev;
    public Cinemachine.CinemachineBrain camera_brain;

    bool is_blending;
    void Start()
    {
        
    }

    void Update()
    {
        var is_prev_blending = is_blending;
        is_blending = camera_brain.IsBlending;

        if(is_prev_blending && !is_blending)
        {
            on_blended_ev.Invoke();
        }
    }
}
