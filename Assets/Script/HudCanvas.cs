using TMPro;
using UnityEngine;

public class HudCanvas : MonoSingleton<HudCanvas>
{
    public GameObject heart_prefab;
    int ui_hp;
    public Transform hp_bar;
    public GameObject result_panel;
    public TextMeshProUGUI frame_rate_text;
    public GameObject frame_rate_ui_root;

    void Start()
    {
#if !DEVELOPMENT_BUILD && !UNITY_EDITOR
        frame_rate_ui_root.SetActive(false);
#endif
    }

    void Update()
    {
#if DEVELOPMENT_BUILD ||UNITY_EDITOR
        float fps = 1.0f / Time.deltaTime;
        frame_rate_text.text = $"{fps:0.}";
#endif
    }

    public void Initialize(int default_hp)
    {
        ui_hp = default_hp;
        for (int i = 0; i < ui_hp; i++)
        {
            Instantiate(heart_prefab, hp_bar.transform);
        }
    }

    public void ChangeHp(int new_hp)
    {
        var delta = ui_hp - new_hp;
        ui_hp = new_hp;
        var hearts = hp_bar.transform.GetComponentsInChildren<HeartUI>();
        for (int i = 0; i < delta; ++i)
        {
            var heart = hearts[hearts.Length - 1 - i];
            heart.Clash();
        }
    }

    public void OnDead()
    {
        result_panel.SetActive(true);
        result_panel.GetComponent<ResultPanel>().Initialize();
    }
}
