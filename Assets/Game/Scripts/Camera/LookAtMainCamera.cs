using Mirror;
using UnityEngine;

public class LookAtMainCamera : MonoBehaviour
{
    private Transform _cameraTransform;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
    }
    
    [ClientCallback]
    private void LateUpdate()
    {
        transform.forward = _cameraTransform.forward;
    }
}
