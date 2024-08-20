using Mirror;
using UnityEngine;

public class GameNetworkManager : NetworkRoomManager
{
    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
    {
        Singleton.Instance.FlagSpawner.Spawn(conn);
        
        return base.OnRoomServerSceneLoadedForPlayer(conn, roomPlayer, gamePlayer);
    }
}
