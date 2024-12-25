using UnityEngine;

public class GamePlayManager : MonoSingleton<GamePlayManager>
{
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    void Update()
    {

    }
}
