using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    // GENERAL REFERENCES
    private InputController _inputController;
    private PlayerInputAnchor _playerInputAnchor;
    private InputActionAsset _inputActionAsset;
    private Camera _mainCamera;
    private Rigidbody _rb;
    
    // INPUT
    private Vector3 MovementOutputVector =>
        ScaleCameraTransform(_mainCamera.transform.forward) * _movementInputVector.y +
        CameraTransformRight * _movementInputVector.x;
    private Vector2 _movementInputVector;
    private Vector2 _lookInputVector;
    private Vector3 _lookOutputVector;
    private Vector3 CameraTransformForward => ScaleCameraTransform(_mainCamera.transform.forward);
    private Vector3 CameraTransformRight => ScaleCameraTransform(_mainCamera.transform.right);
    private Vector3 ScaleCameraTransform(Vector3 cameraTransform) {
        cameraTransform.y = 0.0f;
        cameraTransform.Normalize();
        return cameraTransform;
    }

    [Header("Movement"), Space] 
    [Tooltip("The speed of the player when moving")]
    [SerializeField] private float maxSpeed = 5f;
    
    [Tooltip("How fast the player reaches max speed")]
    [SerializeField] private float acceleration = 200f;
    
    [Tooltip("Limits the maximum force that can be applied whilst accelerating")]
    [SerializeField] private float maxAcceleration = 150;
    
    [Tooltip("Adjusts the acceleration based on how close the current velocity is to the goal velocity. " +
             "By default, this is set to ramp up the acceleration when changing direction, and keep it the same when in the same direction.")]
    [SerializeField] private AnimationCurve accelerationFactor;
    
    [Tooltip("Adjusts the maximum acceleration based on how close the current velocity is to the goal velocity. " +
             "By default, this is set to ramp up the maximum acceleration when changing direction, and keep it the same when in the same direction.")]
    [SerializeField] private AnimationCurve maxAccelerationFactor;
    
    [Tooltip("Used to change which axis the force is applied to.")]
    [SerializeField] private Vector3 forceScale;
    
    [Tooltip("The speed of the player when dashing")]
    [SerializeField] private float dashSpeed = 8f;

    
    [Header("RigidBody Ride"), Space] 
    [Tooltip("How high the player should ride above the ground")]
    [SerializeField] private float rideHeight = 1.2f;
    
    [Tooltip("The strength of the spring force that maintains the ride height")]
    [SerializeField] private float rideSpringStrength = 2000f;
    
    [Tooltip("The damping force that reduces the spring force over time")]
    [SerializeField] private float rideSpringDamper = 100f;
    
    [Tooltip("The layer mask used to determine what is considered ground")]
    [SerializeField] private LayerMask groundMask;
    
    [Tooltip("The origin of the ray used to check for ground")]
    [SerializeField] private Transform groundCheckOrigin;

    private Vector3 _unitGoal;
    private Vector3 _goalVel;
    private bool _isDashing;

    [Header("Interaction")] [Space]
    [SerializeField] private LayerMask interactionLayerMask;
    [SerializeField] private Vector3 interactionBoxSize = new(1, 2, 1);
    private GameObject _currentlyHeldItem;
    [HideInInspector] public GameObject recentlyCastInteractable;

    // ANIMATION
    [HideInInspector] public FloatAnchor playerMotionBlendStateAnchor;
    [HideInInspector] public VoidEventChannelSO playerPickupItemEventChannel;
    [HideInInspector] public VoidEventChannelSO playerDropItemEventChannel;

    public void SetPlayerInput(InputController newInputController) {
        _playerInputAnchor.Provide(newInputController);
    }

    #region Unity Event Methods

    private void Awake() {
        _mainCamera = Camera.main;
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
        Collider hitCollider = default;
        bool hit = CastForInteractable(ref hitCollider);

        if (!hit) return;

        if (_currentlyHeldItem != null) {
            // TODO: Implement Throwing Logic For Entering Throwing Mode
            return;
        }

        if (hitCollider.TryGetComponent(out Workstation station)) {
            if (station.TryGetComponent(out IProduceItem produceItem)) {
                PickupItem(produceItem.ProduceItem());
            }
        }
        
    }

    private void OnItemInteract() {
        Collider hitCollider = default;
        Workstation station;
        bool hit = CastForInteractable(ref hitCollider);

        // Put Down Logic
        if (_currentlyHeldItem != null) {
            // If the player is holding an currentlyStoredItem and the hit collider is a workstation, place the currentlyStoredItem
            if (hit && hitCollider.TryGetComponent(out station) && station.currentlyStoredItem == null) { 
                PlaceItem(station);
                return;
            }

            DropItem();
            return;
        }
        
        // Pick Up Logic
        if (!hit) return;
        if (hitCollider.TryGetComponent(out station)) {
            PickupItem(station);
            return;
        }
        if (hitCollider.TryGetComponent(out Item item)) {
            PickupItem(item.gameObject);
        }
    }

    private void OnDash() {
        StartCoroutine(DashRoutine(0.5f));
    }

    private void FixedUpdate() {
        RigidBodyRide();
        RotateToUpright();
        
        if (!_isDashing) {
            Movement(maxSpeed);
        }
        else {
            Movement(dashSpeed);
        }
        
        QueryInteractables();
    }

    #endregion

    #region Interaction Methods

    private bool CastForInteractable(ref Collider hitCollider) {
        Transform tform = transform;
        // Calculate the center of the box
        Vector3 boxCenter = tform.position + tform.forward;
    
        // Use Physics.OverlapBox to find colliders within the specified box
        Collider[] hitColliders = Physics.OverlapBox(boxCenter, interactionBoxSize, tform.rotation, interactionLayerMask);
    
        // Check if we found any colliders
        if (hitColliders.Length == 0) {
            hitCollider = null;
            return false; // No colliders found
        }
    
        // Find the closest collider
        Collider closestCollider = hitColliders[0];
        float closestDistance = Vector3.Distance(tform.position, closestCollider.transform.position);
        foreach (Collider hit in hitColliders) {
            float distance = Vector3.Distance(tform.position, hit.transform.position);
            if (distance < closestDistance) {
                closestCollider = hit;
                closestDistance = distance;
            }
        }

        hitCollider = closestCollider;
        return true; // Collider found
    }
    
    private void ToggleItemCollisionAndPhysics(GameObject item, bool enable) {
        Rigidbody itemRigidbody = item.GetComponent<Rigidbody>();
        Collider itemCollider = item.GetComponent<Collider>();
        
        if (enable) {
            itemRigidbody.isKinematic = false;
            itemCollider.enabled = true;
            return;
        }

        itemRigidbody.isKinematic = true;
        itemCollider.enabled = false;
    } 

    private void PickupItem(GameObject item) {
        if (_currentlyHeldItem != null) return;

        _currentlyHeldItem = item;
        _currentlyHeldItem.transform.SetParent(transform);
        _currentlyHeldItem.transform.localPosition = new Vector3(0, 0, 0.75f);
        _currentlyHeldItem.transform.localRotation = Quaternion.Euler(0, 0, 0);

        ToggleItemCollisionAndPhysics(_currentlyHeldItem, false);
        
        playerPickupItemEventChannel.RaiseEvent();
    }
    
    private void PickupItem(Workstation station) {
        if (station.currentlyStoredItem == null || _currentlyHeldItem != null) return;

        _currentlyHeldItem = station.OnRemoveItem();
        _currentlyHeldItem.transform.SetParent(transform);
        _currentlyHeldItem.transform.localPosition = new Vector3(0, 0, 0.75f);
        _currentlyHeldItem.transform.localRotation = Quaternion.Euler(0, 0, 0);

        ToggleItemCollisionAndPhysics(_currentlyHeldItem, false);
        
        playerPickupItemEventChannel.RaiseEvent();
    }

    private void DropItem() {
        if (_currentlyHeldItem == null) return;
        
        ToggleItemCollisionAndPhysics(_currentlyHeldItem, true);

        _currentlyHeldItem.transform.SetParent(null);
        _currentlyHeldItem = null;

        playerDropItemEventChannel.RaiseEvent();
    }

    private void PlaceItem(Workstation station) {
        if (station.currentlyStoredItem != null || _currentlyHeldItem == null) return;
        
        ToggleItemCollisionAndPhysics(_currentlyHeldItem, false);
        
        _currentlyHeldItem.transform.SetParent(null);
        station.OnPlaceItem(_currentlyHeldItem);
        _currentlyHeldItem = null;
        
        playerDropItemEventChannel.RaiseEvent();
        
    }
    
    private void QueryInteractables() {
        Collider hitCollider = default;
        bool hit = CastForInteractable(ref hitCollider);

        if (!hit) {
            recentlyCastInteractable = null;
            return;
        }
        
        recentlyCastInteractable = hitCollider.gameObject;
        if (hitCollider.TryGetComponent(out Workstation station)) station.AddHighlight(this);
        if (hitCollider.TryGetComponent(out Item item)) item.AddHighlight(this);
    }

    #endregion

    #region Movement Methods

    private IEnumerator DashRoutine(float durationSeconds) {
        _isDashing = true;
        float timeElapsed = 0;
        while (timeElapsed < durationSeconds) {
            timeElapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        _isDashing = false;
    }

    private void RigidBodyRide() {
        if (!Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, rideHeight + 1, groundMask)) return;

        Vector3 velocity = _rb.velocity;
        Vector3 rayDirection = -transform.up;

        // Get the velocity of the raycastHit rigidbody in case we're riding on a moving platform
        Vector3 hitRigidbodyVelocity = hit.rigidbody ? hit.rigidbody.velocity : Vector3.zero;

        float rayDirectionVelocity = Vector3.Dot(rayDirection, velocity);
        float hitRigidbodyDirectionVelocity = Vector3.Dot(rayDirection, hitRigidbodyVelocity);

        float relativeVelocity = rayDirectionVelocity - hitRigidbodyDirectionVelocity;

        float distanceFromRideHeight = hit.distance - rideHeight;
        float springForce = (distanceFromRideHeight * rideSpringStrength) - (relativeVelocity * rideSpringDamper);

        _rb.AddForce(rayDirection * springForce);
    }

    private void RotateToUpright() {
        Transform tform = transform;

        // The rotationTarget is the rotation that the player should be facing
        Quaternion currentRotation = tform.rotation;
        Quaternion rotationTarget =
            ShortestRotation(Quaternion.LookRotation(tform.forward, tform.up), currentRotation);

        // ToAngleAxis returns the angle and axis of the rotation
        // The out variables are declared inline within the method call
        rotationTarget.ToAngleAxis(out var rotationDegrees, out var rotationAxis);
        rotationAxis.Normalize();

        // Convert the rotation to radians
        float rotationRadians = rotationDegrees * Mathf.Deg2Rad;

        // Apply torque to the RigidBody to rotate the player
        _rb.AddTorque((rotationAxis * (rotationRadians * 500)) - (_rb.angularVelocity * 100));
    }

    private void Movement(float speed) {
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
        Vector3 goalVel = _unitGoal * speed;

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
    void OnDrawGizmos() {
        Transform tform = transform;
        Vector3 boxCenter = tform.position + tform.forward;
    
        // Temporarily store the collider hit by the CastForInteractable method
        Collider hitCollider = null;
        // Check if something was hit and set the Gizmo color accordingly
        bool hitDetected = CastForInteractable(ref hitCollider);
    
        // Set Gizmo color based on whether an interactable was hit
        Gizmos.color = hitDetected ? Color.green : Color.red;
    
        // Draw the box Gizmo
        // Note: Unity's Gizmos.DrawCube expects the world space position (center of the cube) and its size
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, tform.rotation, Vector3.one); // Align Gizmo with the transform's orientation
        Gizmos.DrawWireCube(Vector3.zero, interactionBoxSize * 2); // DrawWireCube draws in local space of the matrix
    }

}