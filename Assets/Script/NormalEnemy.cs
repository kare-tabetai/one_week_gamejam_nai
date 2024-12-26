using UnityEngine;

public class NormalEnemy : MonoBehaviour
{
    public Animator animator;
    public int hp = 100;
    public GameObject brick;
    public GameObject brick_prefab;

    void Start()
    {

    }

    void Update()
    {
        bool walking = false;
        //TODO:
        animator.SetBool("Walking", walking);
    }

    public void Damaged(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            hp = 0;
            animator.SetTrigger("Death");
        }
    }

    public void ActiveBrick()
    {
        brick.SetActive(true);
    }

    public void ShotBrick()
    {
        brick.SetActive(false);
        var brick_shot = Instantiate(
            brick_prefab,
            brick.transform.position,
            Quaternion.identity);
    }
}
