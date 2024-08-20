using UnityEngine;

public class PlayerAppearance : MonoBehaviour
{
    [Header("Mesh Renderer")]
    [SerializeField] private MeshRenderer meshRenderer;
    
    public void OnPlayerColorChanged(Color newPlayerColor)
    {
        meshRenderer.material.color = newPlayerColor;
    }
}
