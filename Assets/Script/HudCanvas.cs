using UnityEngine;

public class HudCanvas : MonoSingleton<HudCanvas>
{
    public GameObject heart_prefab;
    int ui_hp;
    public Transform hp_bar;

    void Start()
    {

    }

    void Update()
    {

    }

    public void Initialize(int default_hp)
    {
        ui_hp = default_hp;
    }

    public void ChangeHp(int new_hp)
    {
        var delta = new_hp - ui_hp;
        var hearts = hp_bar.transform.GetComponentsInChildren<HeartUI>();
        for (int i = 0; i < delta; ++i)
        {
            var heart = hearts[hearts.Length - 1 - i];
            heart.Clash();
        }
    }
}
