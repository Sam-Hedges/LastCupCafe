using System.Collections;
using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    
    private PlayerInputHandler _playerInputHandler;
    private PlayerInputAnchor _playerInputAnchor;

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    private Camera _currentCamera;
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

    private Vector2 _movementInputVector;
    private Vector2 _lookInputVector;
    private Vector3 _lookOutputVector;

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

    public void SetPlayerInput(PlayerInputHandler newPlayerInputHandler) {
        _playerInputAnchor.Provide(newPlayerInputHandler);
    }
    
    public PlayerInputHandler GetPlayerInput() {
        return _playerInputHandler;
    }

    #region Unity Event Methods

    private void Awake() {
        if (mainCamera == null) {
            mainCamera = Camera.main;
        }

        _rb = GetComponent<Rigidbody>();
        _currentCamera = mainCamera;
        
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
        
        // Create new instance on input anchor used to delay the enabling of the player input handler
        _playerInputAnchor = ScriptableObject.CreateInstance<PlayerInputAnchor>();
        _playerInputAnchor.OnAnchorProvided += OnEnable;
    }

    private void OnEnable() {
        if (!_playerInputAnchor.isSet) return;
        _playerInputHandler = _playerInputAnchor.Value;
        if (_playerInputHandler == null) return;
        _playerInputHandler.EnableGameplayInput();
        _playerInputHandler.MoveEvent += OnMovementPlayerInputHandler;
        _playerInputHandler.LookEvent += OnLookPlayerInputHandler;
    }
    
    private void OnDisable() {
        if (_playerInputHandler == null) return;
        _playerInputHandler.DisableAllInput();
        _playerInputHandler.MoveEvent -= OnMovementPlayerInputHandler;
        _playerInputHandler.LookEvent -= OnLookPlayerInputHandler;
    }

    internal void OnMovementPlayerInputHandler(Vector2 input) {
        Vector2 inputValue = new Vector2(input.x, input.y);
        _movementInputVector = inputValue;
    }

    internal void OnLookPlayerInputHandler(Vector2 input) {
        Vector2 inputValue = new Vector2(input.x, input.y);
        _lookInputVector = inputValue;
    }

    private void FixedUpdate() {
        RigidBodyRide();
        RotateToUpright();
        Movement();
    }


    private void Update() {
        //Jump();
    }

    #endregion

    #region Movement Methods

    private void RigidBodyRide() {
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
        // The rotationTarget is the rotation that the player should be facing
        Quaternion currentRotation = transform.rotation;
        Quaternion rotationTarget =
            ShortestRotation(Quaternion.LookRotation(transform.forward, transform.up), currentRotation);
        
        // ToAngleAxis returns the angle and axis of the rotation
        // The out variables are declared inline within the method call
        rotationTarget.ToAngleAxis(out var rotationDegrees, out var rotationAxis); 
        rotationAxis.Normalize();

        // Convert the rotation to radians
        float rotationRadians = rotationDegrees * Mathf.Deg2Rad;

        // Apply torque to the RigidBody to rotate the player
        _rb.AddTorque((rotationAxis * (rotationRadians * 500)) - (_rb.angularVelocity * 100));
    }

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