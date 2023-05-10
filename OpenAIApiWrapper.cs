using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class OpenAIApiWrapper : MonoBehaviour
{
    [SerializeField] private string _apiKey;
    [SerializeField] private string _apiEndpoint = "https://api.openai.com/v1/engines/text-davinci-002/completions";

    // Generate completions using the OpenAI API
    public IEnumerator GenerateCompletion(string prompt, Action<OpenAIResponse> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest request = new UnityWebRequest(_apiEndpoint, "POST"))
        {
            OpenAICompletionRequest requestData = new OpenAICompletionRequest { prompt = prompt, max_tokens = 50 };
            string json = JsonUtility.ToJson(requestData);

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {_apiKey}");
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                onError?.Invoke(request.error + "\n" + request.downloadHandler.text);
            }
            else
            {
                OpenAIResponse response = JsonUtility.FromJson<OpenAIResponse>(request.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
        }
    }

    [Serializable]
    public class OpenAICompletionRequest
    {
        public string prompt;
        public int max_tokens;
    }

    [Serializable]
    public class OpenAIResponse
    {
        public Choice[] choices;
    }

    [Serializable]
    public class Choice
    {
        public string text;
    }
}
