using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    
    public void OnPlayerNumberChanged(int newPlayerNumber)
    {
        nameText.text = $"Player {newPlayerNumber:00}";
    }
}
