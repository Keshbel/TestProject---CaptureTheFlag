using System;
using System.Linq;
using System.Threading.Tasks;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlagSpawner : NetworkBehaviour
{
    [Header("Config")]
    [SerializeField] private FlagConfig flagConfig;
    
    [Header("Prefab")]
    [SerializeField] private Flag flagPrefab;
    [SerializeField] private Transform flagRoot;

    [Header("Plane")] 
    [SerializeField] private MeshRenderer planeMesh;
    
    private float _radius;
    private readonly SyncHashSet<Flag> _flags = new();
    
    public void CheckWinning(Player player)
    {
        var playerFlags = _flags?.Where(flag => flag.TargetOwner == player);
        var isWinning = playerFlags != null && playerFlags.All(flag => flag.HasBeenCaptured);
        
        if (isWinning) SendWinningCmd(player);
    }
    
    #region Commands

    [Command(requiresAuthority = false)]
    private void AddFlag(Flag flag)
    {
        _flags.Add(flag);
    }
    
    [Command(requiresAuthority = false)]
    private async void SendWinningCmd(Player player)
    {
        SendWinningRpc(player);
        await Task.Delay(3000);
        NetworkManager.singleton.ServerChangeScene(NetworkManager.singleton.GetComponent<NetworkRoomManager>().RoomScene);
    }

    #endregion

    #region RPC

    [ClientRpc]
    private void SendWinningRpc(Player player)
    {
        Singleton.Instance.FakeChat.Write("Игрок " + player.playerNumber + " победил!");
        Singleton.Instance.FakeChat.Write("Возвращение в комнату через пару секунд!");
    }

    #endregion"

    #region Server
    
    [Server]
    public async void Spawn(NetworkConnectionToClient networkConnection)
    {
        for (int i = 0; i < flagConfig.FlagsForEachPlayer; i++)
        {
            var flag = Instantiate(flagPrefab, flagRoot);
            var go = flag.gameObject;
            go.SetActive(false);
            
            flag.transform.position = await GetAvailablePosition();
            flag.SetRadius(flagConfig.Radius);
            flag.SetCaptureTime(flagConfig.CaptureTime);
            flag.SetTargetOwner(networkConnection.identity.GetComponent<Player>());
            go.SetActive(true);
            
            NetworkServer.Spawn(go, networkConnection);
            
            AddFlag(flag);

            await Task.Delay(10);
        }
    }
    
    #endregion

    #region Utility
    
    // как альтернатива для не плоской поверхности - можно брать позицию на NavMesh (заранее его подготовив). Но для одной плоской поверхности, как в этом случае, сгодится и такой вариант.
    private async Task<Vector3> GetAvailablePosition()
    {
        var isPositionFound = false;
        var position = new Vector3();
        var attempts = 1000;
        
        while (!isPositionFound)
        {
            if (attempts <= 0) break;
            
            position = GetRandomPointInBounds(planeMesh.bounds);
            if (_flags.All(flag => Vector3.Distance(flag.transform.position, position) >= flagConfig.Radius * 2))
                isPositionFound = true;
            
            attempts--;

            await Task.Yield();
        }

        if (!isPositionFound) throw new Exception("Available position is not found...");
        
        return position;
    }

    private Vector3 GetRandomPointInBounds(Bounds bounds) 
    {
        var minX = bounds.size.x * -0.5f;
        var minY = bounds.size.y * -0.5f;
        var minZ = bounds.size.z * -0.5f;

        return gameObject.transform.TransformPoint(
            new Vector3(Random.Range (minX, -minX),
                Random.Range (minY, -minY),
                Random.Range (minZ, -minZ))
        );
    }
    
    #endregion
}
