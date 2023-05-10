using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCConversationTest : MonoBehaviour
{
    [SerializeField] private OpenAIApiWrapper _openAIApiWrapper;
    [SerializeField] private int _numberOfDialogues = 5;
    [SerializeField] private ChatBubbleController _chatBubbleController;
    [SerializeField] private OpenAIDialogSystem _openAIDialogSystem;

    private List<string> _conversationHistory = new List<string>();

    void Start()
    {
        StartCoroutine(GenerateNPCConversation());
    }

    IEnumerator GenerateNPCConversation()
    {
        for (int i = 0; i < _numberOfDialogues; i++)
        {
            string characterName = _openAIDialogSystem.CurrentCharacterName;
            string characterTraits = _openAIDialogSystem.GetCharacterTraits();

            _conversationHistory.Add($"{characterName} ({characterTraits}):");

            string conversationContext = string.Join("\n", _conversationHistory);

            string prompt = $"Generate a natural conversation between two people with the following traits: {_openAIDialogSystem.GetCharacterTraits()}.\n" +
                            $"Continue the conversation:\n{conversationContext}\n{characterName}: ";

            yield return _openAIApiWrapper.GenerateCompletion(prompt, (response) =>
            {
                string npcResponse = response.choices[0].text.Trim();
                _conversationHistory.Add(npcResponse);
                _chatBubbleController.UpdateChatBubble(npcResponse);

                Debug.Log(npcResponse);
            }, (error) =>
            {
                Debug.LogError("Error generating response: " + error);
            });

            // Adjust the wait time based on the length of the npcResponse or any other criteria.
            yield return new WaitForSeconds(3);
        }
    }
}
