using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultPanel : MonoSingleton<ResultPanel>
{
    public TextMeshProUGUI time_text;
    public TextMeshProUGUI kill_count_text;

    public void Initialize()
    {
        time_text.text = ((int)ScoreManager.Instance.GetTime()).ToString();
        kill_count_text.text = ScoreManager.Instance.kill_count.ToString();
    }

    public void OnClickedToTitleButton()
    {
        SceneManager.LoadScene("MainScene");
    }
}
