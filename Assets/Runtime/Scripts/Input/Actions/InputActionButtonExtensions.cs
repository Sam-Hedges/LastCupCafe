using UnityEngine.InputSystem;

// Used to extend the functionality of the unity input system and use it akin to the 
// legacy system
public static class InputActionButtonExtensions 
{
    // Extends the InputAction class to allow for the use of the legacy input system
    public static bool GetButton(this InputAction action) => action.ReadValue<float>() > 0;
    public static bool GetButtonDown(this InputAction action) => action.triggered && action.ReadValue<float>() > 0;
    public static bool GetButtonUp(this InputAction action) => action.triggered && action.ReadValue<float>() == 0;
    
    // Extends the InputAction.CallbackContext class to allow for the use of the legacy input system
    public static bool GetButton(this InputAction.CallbackContext context) => context.ReadValue<float>() > 0;
    public static bool GetButtonDown(this InputAction.CallbackContext context) => context.performed && context.ReadValue<float>() > 0;
    public static bool GetButtonUp(this InputAction.CallbackContext context) => context.performed && context.ReadValue<float>() == 0;
}
