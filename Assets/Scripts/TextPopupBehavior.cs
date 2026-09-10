using UnityEngine;
using TMPro;

public class TextPopupBehavior : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI displayText;

    [Header("Zone Settings")]
    [TextArea] public string message = "You entered the zone!";
    public string playerTag = "Player";

    void Start()
    {
        if (displayText != null)
            displayText.text = "";
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
            displayText.text = message;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
            displayText.text = "";
    }
}