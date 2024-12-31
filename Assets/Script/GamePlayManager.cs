using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GamePlayManager : MonoSingleton<GamePlayManager>
{
    enum State
    {
        Start,
        MainGame,
    }

    public Cinemachine.CinemachineVirtualCamera title_camera;
    public VendingMachineController player;
    public CinemachineBrain camaera_brain;

    List<EnemySpawner> enemy_spawns = new List<EnemySpawner>();
    State state;

    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // https://discussions.unity.com/t/targetframerate-not-working-in-2021-3-1-lts-webgl/881482
        // WebGLÇæÇ∆vSyncCountÇ0Ç…ÇµÅA60fpsÇ≈ÇÕÇ»Ç≠59fpsÇ…ÇµÇ»Ç¢Ç∆ê≥èÌÇ…ìÆçÏÇµÇ»Ç¢
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 59;
#elif UNITY_EDITOR
        Application.targetFrameRate = 60;
#endif
    }

    void Update()
    {
        if (state == State.Start)
        {
            if (Input.GetMouseButtonDown(0))
            {
                state = State.MainGame;
                //player.Initialize();
                title_camera.Priority = 0;
            }
        }
    }

    public void RegisterSpawner(EnemySpawner spawner)
    {
        enemy_spawns.Add(spawner);
    }

    public void InitializeSpaeners()
    {
        foreach (var spawner in enemy_spawns)
        {
            spawner.Initialize();
        }
    }


    public void ClearSpawner()
    {
        foreach (var spawner in enemy_spawns)
        {
            Destroy(spawner);
        }
    }
}
