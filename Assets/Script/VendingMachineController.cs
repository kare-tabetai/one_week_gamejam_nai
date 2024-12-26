using UnityEngine;
using UnityEngine.InputSystem;

public class VendingMachineController : MonoBehaviour
{
    public Rigidbody rb;
    public StarterAssets.StarterAssetsInputs input;
    public float jump_impulse;
    public float max_speed;
    public float move_force;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;
    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.0f;
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;
    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;
    public LayerMask shot_target_layer;
    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;
    public Transform can_fire_point;
    public GameObject can_bullet;
    public float shot_target_ray_cast_length = 20.0f;
    public float throw_up_y_max = 5.0f;
    public float shot_un_hit_ray_target_length = 5.0f;

#if ENABLE_INPUT_SYSTEM
    public PlayerInput _playerInput;
#endif
    const float _threshold = 0.01f;

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
        }
    }

    float _cinemachineTargetYaw;
    float _cinemachineTargetPitch;

    bool grounded;
    Vector2 input_vec;
    Vector3 shot_target_point;
    void Start()
    {

    }

    void Update()
    {
        rb.WakeUp();

        if (grounded && UnityEngine.Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jump_impulse, ForceMode.Impulse);
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.E) || UnityEngine.Input.GetMouseButtonDown(0))
        {
            shot();
        }

        input_vec = new Vector2(UnityEngine.Input.GetAxisRaw("Horizontal"), UnityEngine.Input.GetAxisRaw("Vertical"));

        calculateShotTargetPoint();
        groundedCheck();
        clampSpeed();
    }

    void shot()
    {
        var can = Instantiate(can_bullet, can_fire_point.position, Random.rotation);
        //var dir = shot_target_point - can.transform.position;
        //dir.Normalize();

        var dir = ThrowUpCalculation.OrbitCalculations(
            can.transform.position,
            shot_target_point,
            shot_target_point.y + throw_up_y_max);
        can.GetComponent<Bullet>().Initialize(dir);
    }

    void LateUpdate()
    {
        CameraRotation();
        correctVendingFornt();
    }

    private void FixedUpdate()
    {
        if (input_vec == Vector2.zero) { return; }

        Vector3 x_dir = CinemachineCameraTarget.transform.right;
        Vector3 y_dir = Vector3.zero;
        var camera_foward = GetMainCameraXZPlaneForward();
        if (camera_foward != null) { y_dir = camera_foward.Value; }

        Vector3 move_dir = x_dir * input_vec.x + y_dir * input_vec.y;
        move_dir.Normalize();
        var force = move_force * Time.deltaTime * move_dir;
        rb.AddForce(force);
    }

    Vector3? GetMainCameraXZPlaneForward()
    {
        var camera_forward = CinemachineCameraTarget.transform.forward;
        camera_forward.y = 0.0f;
        if (camera_forward == Vector3.zero) { return null; }
        camera_forward.Normalize();
        return camera_forward;
    }

    void calculateShotTargetPoint()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, shot_target_ray_cast_length, shot_target_layer))
        {
            shot_target_point = hit.point;
        }
        else
        {
            shot_target_point = Camera.main.transform.position + Camera.main.transform.forward * shot_un_hit_ray_target_length;
        }
    }

    void correctVendingFornt()
    {
        var foward = GetMainCameraXZPlaneForward();
        if (foward != null)
        {
            rb.transform.forward = foward.Value;
        }
    }

    void clampSpeed()
    {
        if (input_vec == Vector2.zero)
        {
            rb.velocity = new Vector3(0.0f, rb.velocity.y, 0.0f);
        }
        rb.angularVelocity = Vector3.zero;

        var vel = rb.velocity;
        var vel_y = vel.y;
        vel.y = 0.0f;
        if (max_speed * max_speed < vel.sqrMagnitude)
        {
            vel = vel.normalized * max_speed;
        }
        rb.velocity = new Vector3(vel.x, vel_y, vel.z);
    }

    private void groundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Debug.DrawLine(transform.position, transform.position - Vector3.down * GroundedOffset);
        grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Gizmos.DrawWireSphere(spherePosition, GroundedRadius);

        Gizmos.DrawWireSphere(shot_target_point, 0.1f);
    }
    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += input.look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += input.look.y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
