using System.Collections.Generic;
using Mirror;

public class PlayerCollection : NetworkBehaviour
{
    [SyncVar] private readonly List<Player> _players = new();
    
    [Command(requiresAuthority = false)]
    public void AddPlayer(Player player)
    {
        if (!_players.Contains(player)) _players.Add(player);
        
        ResetPlayerNumbers();
    }

    [Command(requiresAuthority = false)]
    public void RemovePlayer(Player player)
    {
        _players.Remove(player);
        
        ResetPlayerNumbers();
    }

    [Server]
    private void ResetPlayerNumbers()
    {
        byte number = 0;
        foreach (var player in _players)
        {
            player.playerNumber = number;
            number++;
        }
    }
}
