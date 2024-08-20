using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [field: SerializeField, Header("Virtual Camera")] 
    public CinemachineVirtualCamera VirtualCamera { get; private set; }
    
    public void SetLocalPlayer(Transform player)
    {
        VirtualCamera.Follow = player;
        VirtualCamera.LookAt = player;
    }
}
