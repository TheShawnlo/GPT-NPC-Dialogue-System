using System;
using System.Collections;
using UnityEngine;

public class OpenAIDialogSystem : MonoBehaviour
{
    [SerializeField] private OpenAIApiWrapper _apiWrapper;
    [SerializeField] private string _maleCharacterName = "John";
    [SerializeField] private string _femaleCharacterName = "Alice";
    private string _characterName;
    private string _conversationHistory = "";
    private string[] _characterTraits;

    private void Start()
    {
        _characterName = _maleCharacterName;
        StartCoroutine(GenerateRandomCharacter());
    }

    public string CurrentCharacterName => _characterName;

    public void AddDialog(string message)
    {
        _conversationHistory += $"{_characterName}: {message}\n";
    }

    public IEnumerator GenerateResponse(string characterName, Action<string> onSuccess, Action<string> onError)
    {
        string prompt = _conversationHistory + characterName + ": ";
        yield return _apiWrapper.GenerateCompletion(prompt, (response) =>
        {
            string generatedText = ExtractResponseText(response);
            onSuccess(generatedText);
        }, onError);
    }

    private string ExtractResponseText(OpenAIApiWrapper.OpenAIResponse response)
    {
        if (response.choices != null && response.choices.Length > 0)
        {
            return response.choices[0].text.Trim();
        }
        return "";
    }

    public void SwitchCharacter()
    {
        if (_characterName == _maleCharacterName)
        {
            _characterName = _femaleCharacterName;
        }
        else
        {
            _characterName = _maleCharacterName;
        }
    }
    
    public IEnumerator GenerateRandomCharacter()
    {
        string prompt = $"Generate a list of 3 to 5 traits for a character named {_characterName}, separated by commas.";
        Debug.Log("Generated prompt: " + prompt);

        yield return _apiWrapper.GenerateCompletion(prompt, (response) =>
        {
            string generatedText = ExtractResponseText(response);
            _characterTraits = generatedText.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < _characterTraits.Length; i++)
            {
                _characterTraits[i] = _characterTraits[i].Trim();
            }
            Debug.Log("Generated traits: " + string.Join(", ", _characterTraits));
        }, (error) =>
        {
            Debug.LogError("Error generating response: " + error);
        });
    }
    
public string GetCharacterTraits()
{
    if (_characterTraits != null && _characterTraits.Length > 0)
    {
        return string.Join(", ", _characterTraits);
    }
    else
    {
        return "No character traits available.";
    }
}

}
