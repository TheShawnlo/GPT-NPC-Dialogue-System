using System;
using UnityEngine;
using UnityEngine.UI;
#if ENABLE_WINMD_SUPPORT
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;
#endif

public class TextToSpeechUWP : MonoBehaviour
{
    public void Play(string text)
    {
#if ENABLE_WINMD_SUPPORT
        try
        {
            SpeechSynthesis(text);
        }
        catch (Exception e)
        {
            Debug.LogError("Error in TextToSpeechUWP: " + e.Message);
        }
#endif
    }

#if ENABLE_WINMD_SUPPORT
    private async void SpeechSynthesis(string text)
    {
        var synthesizer = new SpeechSynthesizer();
        using (SpeechSynthesisStream stream = await synthesizer.SynthesizeTextToStreamAsync(text))
        {
            var clip = await ToAudioClip(stream);
            AudioSource.PlayClipAtPoint(clip, Vector3.zero);
        }
    }

    private async System.Threading.Tasks.Task<AudioClip> ToAudioClip(IRandomAccessStream stream)
    {
        var reader = new DataReader(stream.GetInputStreamAt(0UL));
        var bytes = new byte[stream.Size];
        await reader.LoadAsync((uint)stream.Size);
        reader.ReadBytes(bytes);

        const int HEADER_SIZE = 44;
        int dataLength = bytes.Length - HEADER_SIZE;
        float[] samples = new float[dataLength / 2];

        for (int i = 0; i < samples.Length; i++)
        {
            samples[i] = BitConverter.ToInt16(bytes, HEADER_SIZE + i * 2) / (float)Int16.MaxValue;
        }

        var clip = AudioClip.Create("GeneratedTTS", samples.Length, 1, 16000, false);
        clip.SetData(samples, 0);

        return clip;
    }
#endif
}
