using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float spawn_rate;
    public int increse_spawn_count;
    public float decrese_spawn_rate;
    public float minimum_spawn_rate;
    public GameObject[] enemies;
    public Transform enemy_root;

    float timer;
    float spawn_count;
    bool is_initialized;

    private void Start()
    {
        GamePlayManager.Instance.RegisterSpawner(this);
    }

    public void Initialize()
    {
        is_initialized = true;
    }

    void Update()
    {
        if (!is_initialized) { return; }

        timer += Time.deltaTime;
        if (spawn_rate <= timer)
        {
            timer = 0;
            Spawn();

            if (increse_spawn_count <= spawn_count)
            {
                spawn_count = 0;
                spawn_rate -= decrese_spawn_rate;
                spawn_rate = Mathf.Max(spawn_rate, minimum_spawn_rate);
            }
        }
    }

    void Spawn()
    {
        if (!MainCameraController.Instance.IsInCamera(transform.position))
        {
            return;
        }
        var enemy_prefab = enemies[Random.Range(0, enemies.Length - 1)];
        var enemy = Instantiate(enemy_prefab, enemy_root);
        enemy.transform.position = transform.position;
        spawn_count++;
    }
}
