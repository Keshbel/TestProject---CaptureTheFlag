using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NetworkIdentity))]
public class Flag : NetworkBehaviour
{
    [Header("Flag")]
    [SerializeField] private MeshRenderer flagMesh;
    [SerializeField] private SpriteRenderer radiusRenderer;
    [SerializeField] private SpriteRenderer progressRenderer;
    [SerializeField] private GameObject lightObject;

    [field: Header("Data")] 
    [field: SyncVar(hook = nameof(TargetOwnerChanged))] 
    public Player TargetOwner { get; private set; }
    
    [field: Header("States")]
    [field: SyncVar (hook = nameof(HasBeenCapturedHook))] public bool HasBeenCaptured { get; private set; }
    private bool _isWithinRange;
    private bool _isBeingCaptured;
    private bool _isCaptureSuspended;
    private bool _haveLaunchedGame;
    
    [SyncVar] private float _captureTime;
    [SyncVar (hook = nameof(RadiusChanged))] private float _radius;
    
    private CancellationTokenSource _checkTokenSource;
    private CancellationTokenSource _captureTokenSource;
    private Tweener _progressTweener;

    #region Unity Events
    
    private void OnDestroy()
    {
        _checkTokenSource?.Cancel();
        _captureTokenSource?.Cancel();
    }
    
    #endregion

    #region Hooks

    private void TargetOwnerChanged(Player _, Player newPlayer)
    {
        if (_) _.onColorChanged -= SetColor;
        
        if (!newPlayer) return;
        
        SetColor(newPlayer.playerColor);
        newPlayer.onColorChanged += SetColor;
        _progressTweener = progressRenderer.transform.DOMoveY(flagMesh.transform.position.y, 1);
        _progressTweener.Pause();
        
        if (newPlayer.isOwned) CheckingRadiusRoutine();
    }

    private void HasBeenCapturedHook(bool _, bool newBool)
    {
        if (newBool) CaptureTheFlag();
    }

    private void RadiusChanged(float _, float newRadius)
    {
        var diameter = newRadius * 2f;
        radiusRenderer.size = new Vector2(diameter, diameter);
    }

    #endregion

    #region Commands

    [Command (requiresAuthority = false)]
    private void SetHasBeenCaptured(bool isOn)
    {
        HasBeenCaptured = isOn;
    }

    #endregion
    
    #region Public Methods
    
    public void SetTargetOwner(Player player)
    {
        TargetOwner = player;
    }

    public void SetRadius(float radius)
    {
        _radius = radius;
    }

    public void SetCaptureTime(float time)
    {
        _captureTime = time;
    }
    
    #endregion

    #region Private Methods
    
    private void SetColor(Color color)
    {
        flagMesh.material.color = color;
        radiusRenderer.color = color;
    }
    
    private async void CheckingRadiusRoutine()
    {
        _checkTokenSource = new CancellationTokenSource();
        
        while (!_checkTokenSource.IsCancellationRequested)
        {
            _isWithinRange = IsWithinRadius();

            if (_isWithinRange)
            {
                if (!_isBeingCaptured) StartCaptureProcess();
            }
            else if (_isBeingCaptured) StopCaptureProcess();
            
            try { await Task.Delay(100, _checkTokenSource.Token); }
            catch { /* ignore exception */ }
        }
    }

    private void StopCheckingRadius()
    {
        _checkTokenSource?.Cancel();
    }

    private bool IsWithinRadius()
    {
        var flagPosition = transform.position;
        var isWithRadius = _radius >= Vector3.Distance(flagPosition, TargetOwner.transform.position);
        
        return isWithRadius;
    }

    private async void StartCaptureProcess()
    {
        _progressTweener.fullPosition = 0;
        _isBeingCaptured = true;
        _captureTokenSource = new CancellationTokenSource();
        
        var counter = _captureTime;
        while (!_captureTokenSource.IsCancellationRequested)
        {
            if (!_isCaptureSuspended)
            {
                var percent = 1f - (counter / _captureTime);
                _progressTweener.fullPosition = percent;
                
                counter -= Time.deltaTime;
                if (!_haveLaunchedGame) _isCaptureSuspended = 1 >= Random.Range(0, 1001);
                if (_isCaptureSuspended)
                {
                    _haveLaunchedGame = true;
                    Singleton.Instance.SliderGameController.OnGameResult += OnGameResult;
                    Singleton.Instance.SliderGameController.StartMiniGame();
                }
            }
            if (counter <= 0 && !HasBeenCaptured) SetHasBeenCaptured(true);

            await Task.Yield();
        }
    }

    private async void OnGameResult(bool isWin)
    {
        if (isWin) _isCaptureSuspended = false;
        else
        {
            SendLoseMessageCmd();
            
            _isCaptureSuspended = true;
            
            await Task.Delay(3000);

            _isCaptureSuspended = false;
        }
    }

    [Command(requiresAuthority = false)]
    private void SendLoseMessageCmd()
    {
        SendLoseMessageRPC();
    }

    [ClientRpc]
    private void SendLoseMessageRPC()
    {
        Singleton.Instance.FakeChat.Write("Игрок " + TargetOwner.playerNumber + " проиграл в мини-игру!");
    }
    
    private void CaptureTheFlag()
    {
        lightObject.gameObject.SetActive(true);
        radiusRenderer.transform.DOScale(0, 1f);
        
        StopCheckingRadius();
        StopCaptureProcess();

        if (isOwned) Singleton.Instance.FlagSpawner.CheckWinning(TargetOwner);
    }

    private void StopCaptureProcess()
    {
        _haveLaunchedGame = false;
        Singleton.Instance.SliderGameController.HideMiniGame();
        Singleton.Instance.SliderGameController.OnGameResult -= OnGameResult;
        _isBeingCaptured = false;
        _isCaptureSuspended = false;
        _captureTokenSource?.Cancel();
    }
    
    #endregion
}
