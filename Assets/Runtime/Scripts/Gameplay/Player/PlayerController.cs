using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    // INPUT
    private InputController _inputController;
    private PlayerInputAnchor _playerInputAnchor;
    private InputActionAsset _inputActionAsset;

    // CAMERA
    private Camera mainCamera;
    private Vector3 CameraTransformForward => ScaleCameraTransform(mainCamera.transform.forward);
    private Vector3 CameraTransformRight => ScaleCameraTransform(mainCamera.transform.right);

    private Vector3 ScaleCameraTransform(Vector3 cameraTransform) {
        cameraTransform.y = 0.0f;
        cameraTransform.Normalize();
        return cameraTransform;
    }

    private float _xRot;
    private float _yRot;

    // Input
    private Vector3 MovementOutputVector =>
        ScaleCameraTransform(mainCamera.transform.forward) * _movementInputVector.y +
        CameraTransformRight * _movementInputVector.x;

    private Vector2 _movementInputVector;
    private Vector2 _lookInputVector;
    private Vector3 _lookOutputVector;


    // MOVEMENT
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
    private GameObject _currentlyHeldItem;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckRad = 0.5f;
    [SerializeField] private Transform groundCheckOrigin;


    [Header("Interaction")] [SerializeField]
    private LayerMask interactionLayerMask;

    [SerializeField] private Vector3 interactionBoxSize = new(1, 2, 1);
    [SerializeField] private float interactionBoxCastMaxDistance = 1f;
    private bool _hitDetect;
    private RaycastHit _hit;

    // ANIMATION
    [HideInInspector] public FloatAnchor playerMotionBlendStateAnchor;
    [HideInInspector] public VoidEventChannelSO playerPickupItemEventChannel;
    [HideInInspector] public VoidEventChannelSO playerDropItemEventChannel;


    public void SetPlayerInput(InputController newInputController) {
        _playerInputAnchor.Provide(newInputController);
    }

    #region Unity Event Methods

    private void Awake() {
        mainCamera = Camera.main;
        _rb = GetComponent<Rigidbody>();

        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;

        // Create new instance on input anchor used to delay the enabling of the player input handler
        _playerInputAnchor = ScriptableObject.CreateInstance<PlayerInputAnchor>();
        _playerInputAnchor.OnAnchorProvided += OnEnable;
    }

    private void OnEnable() {
        if (!_playerInputAnchor.isSet) return;
        _inputController = _playerInputAnchor.Value;
        if (_inputController == null) return;

        _inputController.EnableGameplayInput();
        _inputController.MoveEvent += OnMovement;
        _inputController.DashEvent += OnDash;
        _inputController.ItemInteractEvent += OnItemInteract;
        _inputController.StationInteractEvent += OnStationInteract;
        _inputController.PauseEvent += OnPause;
        _inputController.EmoteEvent += OnEmote;
    }


    private void OnDisable() {
        if (_inputController == null) return;

        _inputController.DisableAllInput();
        _inputController.MoveEvent -= OnMovement;
        _inputController.DashEvent -= OnDash;
        _inputController.ItemInteractEvent -= OnItemInteract;
        _inputController.StationInteractEvent -= OnStationInteract;
        _inputController.PauseEvent -= OnPause;
        _inputController.EmoteEvent -= OnEmote;
    }

    internal void OnMovement(Vector2 input) {
        Vector2 inputValue = new Vector2(input.x, input.y);
        _movementInputVector = inputValue;

        // Broadcast the motion blend state to the animation controller
        playerMotionBlendStateAnchor.Provide(_movementInputVector.magnitude);
    }

    private void OnEmote() {
        throw new System.NotImplementedException();
    }

    private void OnPause() {
        throw new System.NotImplementedException();
    }

    private void OnStationInteract() {
        if (Physics.BoxCast(transform.position + transform.forward, interactionBoxSize, transform.forward,
                out RaycastHit hit, transform.rotation, interactionBoxCastMaxDistance, interactionLayerMask)) {
            if (_currentlyHeldItem != null) {
                // TODO: Implement Throwing Logic For Entering Throwing Mode
                return;
            }

            if (!hit.collider.CompareTag("Station")) return;
            if (hit.collider.TryGetComponent(out IInteractable interactable)) interactable?.OnInteract();
        }
    }

    private void OnThrow() {
        throw new System.NotImplementedException();
    }

    private void OnItemInteract() {
        Debug.Log("Item Interact");
        _hitDetect = Physics.BoxCast(transform.position, interactionBoxSize, transform.forward,
            out _hit, transform.rotation, interactionBoxCastMaxDistance, interactionLayerMask);

        // Put Down Logic
        if (_currentlyHeldItem != null) {
            if (_hitDetect) {
                if (_hit.collider.TryGetComponent(out Station station)) {
                    PlaceItem();
                    return;
                }
            }

            DropItem();
            return;
        }

        // Pick Up Logic
        if (!_hitDetect) return;
        if (_hit.collider.TryGetComponent(out Item item)) {
            PickupItem(_hit.collider.gameObject);
            return;
        }
    }

    private void OnDash() {
        // Implement Dash Logic
        // Use my movement code to implement a dash
    }

    private void FixedUpdate() {
        RigidBodyRide();
        RotateToUpright();
        Movement();
    }

    #endregion

    #region Interaction Methods

    private void PickupItem(GameObject item) {
        if (_currentlyHeldItem != null) return;
        Debug.Log("Picking Up Item");

        _currentlyHeldItem = item;
        _currentlyHeldItem.transform.SetParent(transform);
        _currentlyHeldItem.transform.localPosition = new Vector3(0, 0, 0.75f);
        
        // Disable the rigidbody of the item
        Rigidbody itemRigidbody = _currentlyHeldItem.GetComponent<Rigidbody>();
        itemRigidbody.isKinematic = true;
        
        // Disable the collider of the item
        Collider itemCollider = _currentlyHeldItem.GetComponent<Collider>();
        itemCollider.enabled = false;
        
        // Broadcast the pickup event to the player pickup item event channel
        playerPickupItemEventChannel.RaiseEvent();
    }

    private void DropItem() {
        if (_currentlyHeldItem == null) return;
        Debug.Log("Dropping Item");

        _currentlyHeldItem.transform.SetParent(null);
        
        // Enable the collider of the item
        Rigidbody itemRigidbody = _currentlyHeldItem.GetComponent<Rigidbody>();
        itemRigidbody.isKinematic = false;
        
        // Enable the collider of the item
        Collider itemCollider = _currentlyHeldItem.GetComponent<Collider>();
        itemCollider.enabled = true;

        _currentlyHeldItem = null;
        
        playerDropItemEventChannel.RaiseEvent();
    }

    private void PlaceItem() {
        if (_currentlyHeldItem == null) return;
        Debug.Log("Placing Item");

        _currentlyHeldItem.transform.SetParent(null);
        _currentlyHeldItem = null;
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
        // Clamp the magnitude of the movement output vector to 1 to ensure the direction doesn't exceed a unit vector.
        Vector3 moveDirection = Vector3.ClampMagnitude(MovementOutputVector, 1f);

        // Set the goal direction for the unit based on the clamped movement direction.
        _unitGoal = moveDirection;

        // Make the player face the direction of the movement.
        if (moveDirection != Vector3.zero) {
            // Slerp the player's rotation to the direction of movement.
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(moveDirection), Time.fixedDeltaTime * 25f);
            
            // transform.rotation = Quaternion.LookRotation(moveDirection);
        }

        // Normalize the goal velocity to get the unit direction vector of velocity.
        Vector3 unitVel = _goalVel.normalized;

        // Calculate the dot product between the unit goal direction and the unit velocity vector.
        float velDot = Vector3.Dot(_unitGoal, unitVel);

        // Determine the acceleration based on the dot product, adjusted by an acceleration factor curve.
        float accel = acceleration * accelerationFactor.Evaluate(velDot);

        // Calculate the desired velocity in the goal direction, scaled by the maximum speed.
        Vector3 goalVel = _unitGoal * maxSpeed;

        // Gradually adjust the current velocity towards the desired velocity using the calculated acceleration,
        // taking into account the fixed delta time for consistent physics updates.
        _goalVel = Vector3.MoveTowards(_goalVel, goalVel, accel * Time.fixedDeltaTime);

        // Calculate the target acceleration needed to reach the goal velocity from the current velocity.
        Vector3 targetAccel = (_goalVel - _rb.velocity) / Time.fixedDeltaTime;

        // Determine the maximum allowable acceleration.
        float maxAccel = maxAcceleration * maxAccelerationFactor.Evaluate(velDot);

        // Clamp the target acceleration.
        targetAccel = Vector3.ClampMagnitude(targetAccel, maxAccel);

        // Apply a scaled force to the rigidbody to achieve the target acceleration,
        // scaled by the rigidbody's mass and an additional force scaling factor.
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

    // Gizmo function to draw the interaction box
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;

        //Check if there has been a hit yet
        if (_hitDetect) {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(transform.position, transform.forward * _hit.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(transform.position + transform.forward * _hit.distance, interactionBoxSize * 2);
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(transform.position, transform.forward * interactionBoxCastMaxDistance);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(transform.position + transform.forward * interactionBoxCastMaxDistance,
                interactionBoxSize * 2);
        }
    }
}