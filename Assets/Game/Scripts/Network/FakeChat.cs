using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FakeChat : MonoBehaviour
{
    [Header("Content Panel")] 
    [SerializeField] private Transform contentRoot;
    
    [Header("Prefab")]
    [SerializeField] private TMP_Text messagePrefab;

    public void Write(string text)
    {
        var instanceText = Instantiate(messagePrefab, contentRoot);
        instanceText.text = text;
        LayoutRebuilder.ForceRebuildLayoutImmediate(instanceText.transform.parent.GetComponent<RectTransform>());
    }
}
