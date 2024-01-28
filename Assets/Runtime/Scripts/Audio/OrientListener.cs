using UnityEngine;

// Orient the listener to point in the same direction as the camera.
public class OrientListener : MonoBehaviour
{
    // Reference to the camera transform determine listener orientation
	[SerializeField] private TransformAnchor cameraTransform;

    void LateUpdate()
    {
		if(cameraTransform.isSet)
	        transform.forward = cameraTransform.Value.forward;
    }
}
