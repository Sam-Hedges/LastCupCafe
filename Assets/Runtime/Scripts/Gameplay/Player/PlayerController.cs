using System.Collections;
using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] private InputHandler input;
    private PlayerInputHandler _playerInputHandler;

    [Header("Camera")] [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    private CinemachineVirtualCamera _currentCamera;
    private Vector3 CameraTransformForward => ScaleCameraTransform(_currentCamera.transform.forward);
    private Vector3 CameraTransformRight => ScaleCameraTransform(_currentCamera.transform.right);

    private Vector3 ScaleCameraTransform(Vector3 cameraTransform) {
        cameraTransform.y = 0.0f;
        cameraTransform.Normalize();
        return cameraTransform;
    }

    private float _xRot;
    private float _yRot;

    // Input
    private Vector3 MovementOutputVector =>
        ScaleCameraTransform(_currentCamera.transform.forward) * _movementInputVector.y +
        CameraTransformRight * _movementInputVector.x;

    private bool IsMovementPressed => _movementInputVector != Vector2.zero;
    private Vector2 _movementInputVector;
    private Vector2 _lookInputVector;
    private Vector3 _lookOutputVector;
    private bool IsLookPressed => _lookInputVector != Vector2.zero;

    [HideInInspector]
    public struct LocalTransform {
        public Vector3 Up;
        public Vector3 Forward;
        public Vector3 Right;
    }

    [HideInInspector] public LocalTransform localTransform;
    private Rigidbody _rb;
    [SerializeField] private float rideHeight = 1.2f;
    [SerializeField] private float rideSpringStrength = 2000f;
    [SerializeField] private float rideSpringDamper = 100f;
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float acceleration = 200f;
    [SerializeField] private AnimationCurve accelerationFactor;
    [SerializeField] private float maxAcceleration = 150;
    [SerializeField] private AnimationCurve maxAccelerationFactor;
    [SerializeField] private Vector3 forceScale;
    private Vector3 _unitGoal;
    private Vector3 _goalVel;
    private bool IsGrounded => Physics.CheckSphere(groundCheckOrigin.position, groundCheckRad, groundMask);
    private Vector3 _velocity;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckRad = 0.5f;
    [SerializeField] private Transform groundCheckOrigin;

    public void SetPlayerInput(PlayerInputHandler playerInputHandler) {
        _playerInputHandler = playerInputHandler;
    }
    
    public PlayerInputHandler GetPlayerInput() {
        return _playerInputHandler;
    }
    
    private void InitializeCameras() {
        _currentCamera = playerCamera;
    }

    private void InitializeLocalTransform() {
        Transform t = transform;
        localTransform.Up = t.up;
        localTransform.Forward = t.forward;
        localTransform.Right = t.right;
    }

    #region Unity Event Methods

    private void Initialise() {

        if (mainCamera == null) {
            mainCamera = Camera.main;
        }

        _rb = GetComponent<Rigidbody>();
        InitializeLocalTransform();
        InitializeCameras();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void Start() {
        Initialise();
    }
    
    private void OnEnable() {
        input.EnableGameplayInput();
        input.MoveEvent += OnMovementInput;
        input.LookEvent += OnLookInput;
    }

    private void OnDisable() {
        input.DisableAllInput();
        input.MoveEvent -= OnMovementInput;
        input.LookEvent -= OnLookInput;
    }

    internal void OnMovementInput(Vector2 input) {
        Vector2 inputValue = new Vector2(input.x, input.y);
        _movementInputVector = inputValue;
    }

    internal void OnLookInput(Vector2 input) {
        Vector2 inputValue = new Vector2(input.x, input.y);
        _lookInputVector = inputValue;
    }

    private void FixedUpdate() {
        RigidbodyRide();
        RotateToUpright();
        RotateCamera();
        Movement();
    }


    private void Update() {
        //Jump();
    }

    #endregion

    #region Movement Methods

    private void RigidbodyRide() {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, rideHeight + 1)) {
            Vector3 velocity = _rb.velocity;
            Vector3 rayDirection = -transform.up;

            Vector3 otherVelocity = Vector3.zero;
            Rigidbody hitRigidbody = hit.rigidbody;
            if (hitRigidbody != null) {
                otherVelocity = hitRigidbody.velocity;
            }

            float rayDirectionVelocity = Vector3.Dot(rayDirection, velocity);
            float otherDirectionVelocity = Vector3.Dot(rayDirection, otherVelocity);

            float relativeVelocity = rayDirectionVelocity - otherDirectionVelocity;

            float x = hit.distance - rideHeight;

            float springForce = (x * rideSpringStrength) - (relativeVelocity * rideSpringDamper);

            Debug.DrawLine(transform.position, transform.position + (-transform.up * (rideHeight + 1)), Color.red);

            _rb.AddForce(rayDirection * springForce);
        }
    }

    private void RotateToUpright() {
        Quaternion currentRotation = transform.rotation;
        Quaternion rotationTarget =
            ShortestRotation(Quaternion.LookRotation(transform.forward, transform.up), currentRotation);

        Vector3 rotationAxis;
        float rotationDegrees;

        rotationTarget.ToAngleAxis(out rotationDegrees, out rotationAxis);
        rotationAxis.Normalize();

        float rotationRadians = rotationDegrees * Mathf.Deg2Rad;

        _rb.AddTorque((rotationAxis * (rotationRadians * 500)) - (_rb.angularVelocity * 100));
    }

    private void RotateCamera() {
        _xRot -= _lookInputVector.y * 100f * Time.fixedDeltaTime;
        _xRot = Mathf.Clamp(_xRot, -90f, 90f);
        _yRot += _lookInputVector.x * 100f * Time.fixedDeltaTime;

        playerCamera.transform.localRotation = Quaternion.Euler(_xRot, 0, 0);

        transform.rotation = Quaternion.Euler(0, _yRot, 0);
    }

    // Applies movement to the player character based on the players input
    private void Movement() {
        // Calculate final movement Vector
        Vector3 moveDirection = Vector3.ClampMagnitude(MovementOutputVector, 1f);

        _unitGoal = moveDirection;

        Vector3 unitVel = _goalVel.normalized;
        float velDot = Vector3.Dot(_unitGoal, unitVel);
        float accel = acceleration * accelerationFactor.Evaluate(velDot);
        Vector3 goalVel = _unitGoal * maxSpeed;
        _goalVel = Vector3.MoveTowards(_goalVel, goalVel, accel * Time.fixedDeltaTime);

        Vector3 targetAccel = (_goalVel - _rb.velocity) / Time.fixedDeltaTime;
        float maxAccel = maxAcceleration * maxAccelerationFactor.Evaluate(velDot);
        targetAccel = Vector3.ClampMagnitude(targetAccel, maxAccel);
        _rb.AddForce(Vector3.Scale(targetAccel * _rb.mass, forceScale));
    }


    private void Jump() {
        // Changes the height position of the player..
        // if (_input.Player.Jump.GetButtonDown() && IsGrounded)
        // {
        //     _velocity.y += Mathf.Sqrt(playerJumpHeight * -3.0f * gravityStrength);
        // }
    }

    #endregion

    private static Quaternion ShortestRotation(Quaternion to, Quaternion from) {
        if (Quaternion.Dot(to, from) < 0) {
            return to * Quaternion.Inverse(Multiply(from, -1));
        }

        else return to * Quaternion.Inverse(from);
    }

    private static Quaternion Multiply(Quaternion input, float scalar) {
        return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
    }
}