using System;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class Player : NetworkBehaviour
{
    public Action<Color> onColorChanged;
    
    [Header("Components")] 
    [SerializeField]
    private PlayerUI playerUI;

    [SerializeField] 
    private PlayerAppearance playerAppearance;

    #region SyncVars

    [Header("SyncVars")] 
    [SyncVar(hook = nameof(PlayerNumberChanged))]
    public int playerNumber;

    [SyncVar(hook = nameof(PlayerColorChanged))]
    public Color playerColor = Color.white;
    
    private void PlayerNumberChanged(int _, int newPlayerNumber)
    {
        playerUI.OnPlayerNumberChanged(newPlayerNumber);
        
        var colorNumber = Mathf.Clamp(newPlayerNumber, 0, Enum.GetValues(typeof(PlayerColors)).Length - 1);
        ColorUtility.TryParseHtmlString(((PlayerColors)colorNumber).ToString(), out var color);
        
        SetColor(color);
    }
    
    private void PlayerColorChanged(Color _, Color newPlayerColor)
    {
        playerAppearance.OnPlayerColorChanged(newPlayerColor);
        
        onColorChanged?.Invoke(newPlayerColor);
    }

    #endregion
    
    #region Client

    [Command]
    private void SetColor(Color color)
    {
        playerColor = color;
    }
    
    public override void OnStartLocalPlayer()
    {
        Singleton.Instance.CameraController.SetLocalPlayer(transform);
        
        Singleton.Instance.PlayerCollection.AddPlayer(this);
    }

    public override void OnStopLocalPlayer()
    {
        Singleton.Instance.PlayerCollection.RemovePlayer(this);
        
        base.OnStopLocalPlayer();
    }

    #endregion
}
