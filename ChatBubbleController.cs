using TMPro;
using UnityEngine;

public class ChatBubbleController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _chatText;

    public void UpdateChatBubble(string text)
    {
        _chatText.text = text;
    }
}
