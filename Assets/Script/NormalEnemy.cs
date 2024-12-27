using UnityEngine;

public class NormalEnemy : MonoBehaviour
{
    public enum State
    {
        Idle,
        TrackingToShootPoint,
        TrackingToAttackPoint,
        Shooting,
        ShootingInterval,
        Attacking,
        AttackingInterval,
    }

    public Animator animator;
    public Rigidbody rb;
    public int hp = 100;
    public GameObject brick;
    public GameObject brick_prefab;
    public float brick_shoot_range;
    public float attack_range;
    public Transform target;
    public float tracking_speed;
    public float throw_up_y_max = 1.0f;

    State state;
    float timer;

    void Update()
    {
        if (hp <= 0) { return; }

        switch (state)
        {
            case State.Idle:
                UpdateIdle();
                break;
            case State.TrackingToShootPoint:
                UpdateShootPointTracking();
                break;
            case State.TrackingToAttackPoint:
                UpdateAttackPointTracking();
                break;
            case State.Shooting:
                UpdateShooting();
                break;
            case State.Attacking:
                UpdateAttacking();
                break;
        }

    }

    void UpdateIdle()
    {
        if (target)
        {
            state = Random.Range(0, 2) == 0 ? State.TrackingToShootPoint : State.TrackingToAttackPoint;
            animator.SetBool("Walking", true);
        }
        else
        {
            animator.SetBool("Walking", false);
        }
    }

    void UpdateShootPointTracking()
    {
        var to_target = target.position - transform.position;
        if (to_target.sqrMagnitude <= brick_shoot_range * brick_shoot_range)
        {
            ChangeStateShoot();
            return;
        }

        to_target.Normalize();
        rb.velocity = Vector3.zero;
    }

    void ChangeStateShoot()
    {
        state = State.Shooting;
        animator.SetTrigger("Shoot");
        rb.velocity = Vector3.zero;
    }

    void UpdateAttackPointTracking()
    {
        var to_target = target.position - transform.position;
        if (to_target.sqrMagnitude <= attack_range * attack_range)
        {
            ChangeStateAttack();
            return;
        }

        to_target.Normalize();
        rb.velocity = Vector3.zero;
    }

    void ChangeStateAttack()
    {
        state = State.Attacking;
        animator.SetTrigger("Attack");
        rb.velocity = Vector3.zero;
    }

    void UpdateShooting()
    {

    }

    void UpdateAttacking()
    {

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
        if (!target) { return; }

        brick.SetActive(false);
        var brick_shot = Instantiate(
            brick_prefab,
            brick.transform.position,
            Quaternion.identity);

        var dir = ThrowUpCalculation.OrbitCalculations(
            brick_shot.transform.position,
            target.position,
            target.position.y + throw_up_y_max);
        brick_shot.GetComponent<Bullet>().Initialize(dir);
    }

    public void OnEnterPlayer(Collider2D col)
    {
        target = col.transform;
    }

    public void OnExitPlayer(Collider2D col)
    {
        target = null;
    }
}
