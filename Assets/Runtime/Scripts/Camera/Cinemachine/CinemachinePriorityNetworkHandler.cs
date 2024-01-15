using UnityEngine;
using Unity.Netcode;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CinemachinePriorityNetworkHandler : NetworkBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;
    
    private void SetCameraPriority(int priority)
    {
        _virtualCamera.Priority = priority;
    }
    
    public override void OnNetworkSpawn()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        
        if (IsOwner)
        {
            SetCameraPriority(10);
        }
        else
        {
            SetCameraPriority(0);
        }
        
        base.OnNetworkSpawn();
    }
}
