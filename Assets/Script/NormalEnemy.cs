using UnityEngine;

public class NormalEnemy : MonoBehaviour
{
    public enum State
    {
        Idle,
        TrackingToShootPoint,
        Shooting,
        ShootingInterval,
        Dead,
    }

    public enum TrackingType
    {
        Default,
        RandomAround,
        Back,
    }

    public Animator animator;
    public Rigidbody rb;
    public int hp = 100;
    public GameObject brick;
    public GameObject brick_prefab;
    public float brick_shoot_range;
    public VendingMachineController target;
    public float tracking_speed;
    public float throw_up_y_max = 1.0f;
    public float shot_interval;
    public float dead_erase_interval;
    public float point_track_give_up_time = 8.0f;
    public float random_around_tracking_range = 5.0f;
    public TrackingType tracking_type;
    public bool is_auto_target;
    public Transform target_point;
    public Vector3 TargetPoint { get { return target_point.position; } }

    Vector3 tracking_point;
    State state;
    float timer;

    private void Start()
    {
        if (is_auto_target)
        {
            target = VendingMachineController.s_controller;
        }
    }

    void Update()
    {
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
            case State.Dead:
                UpdateDead();
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
        switch (tracking_type)
        {
            case TrackingType.Default:
                break;
            case TrackingType.RandomAround:
                var random_range = Random.insideUnitCircle * random_around_tracking_range;
                tracking_point = target.TargetPoint + new Vector3(random_range.x, 0.0f, random_range.y);
                break;
            case TrackingType.Back:
                var to_target = target.TargetPoint - TargetPoint;
                tracking_point = target.TargetPoint + to_target;
                break;
        }
    }

    void UpdateShootPointTracking()
    {
        if (!target)
        {
            state = State.Idle;
            return;
        }

        switch (tracking_type)
        {
            case TrackingType.Default:
                UpdateTargetTracking();
                break;
            case TrackingType.RandomAround:
            case TrackingType.Back:
                UpdatePointTracking();
                break;
        }
    }

    void UpdateTargetTracking()
    {
        var to_target = target.TargetPoint - transform.position;
        if (to_target.sqrMagnitude <= brick_shoot_range * brick_shoot_range)
        {
            ChangeStateShoot();
            return;
        }

        to_target.y = 0;
        if (to_target == Vector3.zero) { return; }

        to_target.Normalize();
        transform.forward = to_target;
        var delta = Time.deltaTime * tracking_speed * to_target;
        transform.Translate(delta, Space.World);
    }

    void UpdatePointTracking()
    {
        timer += Time.deltaTime;
        if (point_track_give_up_time <= timer)
        {
            timer = 0;
            ChangeTracking();
        }

        var to_target = tracking_point - transform.position;
        to_target.y = 0;
        if (to_target == Vector3.zero)
        {
            ChangeStateShoot();
            return;
        }
        float delta_value = Time.deltaTime * tracking_speed;
        if (to_target.magnitude <= delta_value)
        {
            ChangeStateShoot();
            return;
        }
        
        var delta_vector = delta_value * to_target.normalized;
        transform.forward = to_target.normalized;
        transform.Translate(delta_vector, Space.World);
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
        var to_target = target.TargetPoint - transform.position;
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

    void UpdateDead()
    {
        timer += Time.deltaTime;
        if (dead_erase_interval <= timer)
        {
            timer = 0;
            Destroy(gameObject);
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
            state = State.Dead;
            ScoreManager.Instance.AddKillScore();
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
            target.TargetPoint,
            target.TargetPoint.y + throw_up_y_max);
        brick_shot.GetComponent<Bullet>().Initialize(dir);
    }

    public void OnEnterPlayer(Collider col)
    {
        target = col.GetComponent<VendingMachineController>();
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
            Gizmos.DrawLine(transform.position, target.TargetPoint);
            Gizmos.DrawLine(transform.position, tracking_point);
        }
    }
}
