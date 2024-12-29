using Cinemachine;
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
}
