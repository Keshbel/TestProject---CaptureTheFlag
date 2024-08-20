using UnityEngine;

[CreateAssetMenu(fileName = "FlagConfig", menuName = "Configs/FlagConfig")]
public class FlagConfig : ScriptableObject
{
    [field: Header("Settings")]
    [field: SerializeField] public float Radius { get; private set; }
    [field: SerializeField] public int FlagsForEachPlayer { get; private set; }
    [field: SerializeField] public float CaptureTime { get; private set; }
}
