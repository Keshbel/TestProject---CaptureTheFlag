using UnityEngine;

public class Singleton : MonoBehaviour
{
    private PlayerCollection _playerCollection;
    public PlayerCollection PlayerCollection => ReturnObject(ref _playerCollection);
    
    private CameraController _cameraController;
    public CameraController CameraController => ReturnObject(ref _cameraController);
    
    private JoystickContainer _joystickContainer;
    public JoystickContainer JoystickContainer => ReturnObject(ref _joystickContainer);

    private SliderGameController _sliderGameController;
    public SliderGameController SliderGameController => ReturnObject(ref _sliderGameController);

    private FlagSpawner _flagSpawner;
    public FlagSpawner FlagSpawner => ReturnObject(ref _flagSpawner);

    private FakeChat _fakeChat;
    public FakeChat FakeChat => ReturnObject(ref _fakeChat);
    
    #region Singleton
    
    private static Singleton _instance;
    public static Singleton Instance => ReturnObject(ref _instance);

    #endregion

    #region Utility
    
    private static T ReturnObject<T>(ref T component) where T : Component
    {
        if (!component) component = FindObjectOfType<T>(true);
        return component;
    }
    
    #endregion
}
