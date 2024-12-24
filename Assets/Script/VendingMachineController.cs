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
    public float GroundedOffset = -0.14f;
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;
    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;
    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;
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

    bool Grounded;
    Vector2 input_vec;
    void Start()
    {

    }

    void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jump_impulse, ForceMode.Impulse);
        }

        input_vec = new Vector2(UnityEngine.Input.GetAxisRaw("Horizontal"), UnityEngine.Input.GetAxisRaw("Vertical"));

        GroundedCheck();
    }

    void LateUpdate()
    {
        CameraRotation();
    }

    private void FixedUpdate()
    {
        rb.WakeUp();
        if (input_vec == Vector2.zero)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        var force = move_force * Time.deltaTime * new Vector3(input_vec.x, 0, input_vec.y);

        rb.AddForce(force);

        var vel = rb.velocity;
        if (max_speed * max_speed < vel.sqrMagnitude)
        {
            vel = vel.normalized * max_speed;
        }
        rb.velocity = vel;
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

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
