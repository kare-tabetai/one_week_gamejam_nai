using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

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
        Application.targetFrameRate = 60;
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
