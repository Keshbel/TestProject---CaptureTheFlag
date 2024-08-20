using UnityEngine;

public class JoystickContainer : MonoBehaviour
{
    [field: Header("Joysticks")]
    [field: SerializeField] 
    public DynamicJoystick JoystickMovement { get; private set; }
}
