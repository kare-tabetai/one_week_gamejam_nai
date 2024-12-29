using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainCameraController : MonoSingleton<MainCameraController>
{
    public UnityEvent on_blended_ev;
    public Cinemachine.CinemachineBrain camera_brain;
    public Camera _camera;

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

    public bool IsInCamera(Vector3 point)
    {
        Vector3 viewportPos = _camera.WorldToViewportPoint(point);

        // ビューポート座標の範囲は (0, 0) ～ (1, 1)
        bool isInView = viewportPos.x >= 0 && viewportPos.x <= 1 &&
                        viewportPos.y >= 0 && viewportPos.y <= 1 &&
                        viewportPos.z > 0;
        return isInView;
    }
}
