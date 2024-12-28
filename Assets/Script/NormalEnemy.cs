using UnityEngine;

public class NormalEnemy : MonoBehaviour
{
    public enum State
    {
        Idle,
        TrackingToShootPoint,
        Shooting,
        ShootingInterval,
    }

    public Animator animator;
    public Rigidbody rb;
    public int hp = 100;
    public GameObject brick;
    public GameObject brick_prefab;
    public float brick_shoot_range;
    public Transform target;
    public float tracking_speed;
    public float throw_up_y_max = 1.0f;
    public float shot_interval;

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
            case State.Shooting:
                FaceToTarget();
                break;
            case State.ShootingInterval:
                UpdateShootingInterval();
                break;
        }

    }

    void UpdateIdle()
    {
        if (target)
        {
            ChangeTracking();
        }
        else
        {
            animator.SetBool("Walking", false);
        }
    }

    void ChangeTracking()
    {
        state = State.TrackingToShootPoint;
        animator.SetBool("Walking", true);
    }

    void UpdateShootPointTracking()
    {
        if (!target)
        {
            state = State.Idle;
            return;
        }

        var to_target = target.position - transform.position;
        if (to_target.sqrMagnitude <= brick_shoot_range * brick_shoot_range)
        {
            ChangeStateShoot();
            return;
        }

        to_target.y = 0; ;
        if (to_target == Vector3.zero) { return; }

        to_target.Normalize();
        transform.forward = to_target;
        var delta = Time.deltaTime * tracking_speed * to_target;
        transform.Translate(delta, Space.World);
    }

    void ChangeStateShoot()
    {
        state = State.Shooting;
        animator.SetTrigger("Shoot");
        animator.SetBool("Walking", false);
        FaceToTarget();
    }

    void FaceToTarget()
    {
        if (!target) { return; }
        var to_target = target.position - transform.position;
        to_target.y = 0; ;
        if (to_target == Vector3.zero) { return; }
        to_target.Normalize();
        transform.forward = to_target;
    }

    void UpdateShootingInterval()
    {
        timer += Time.deltaTime;
        if (shot_interval <= timer)
        {
            timer = 0;
            ChangeTracking();
        }
    }

    public void EndShoot()
    {
        state = State.ShootingInterval;
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

    public void OnEnterPlayer(Collider col)
    {
        target = col.transform;
    }

    public void OnExitPlayer(Collider col)
    {
        target = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, brick_shoot_range);
        if (target)
        {
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}
