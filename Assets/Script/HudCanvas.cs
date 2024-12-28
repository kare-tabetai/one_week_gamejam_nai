using UnityEngine;

public class HudCanvas : MonoSingleton<HudCanvas>
{
    public GameObject heart_prefab;
    int ui_hp;

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

    }
}
