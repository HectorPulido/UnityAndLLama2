using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public class LlamaCharacter : MonoBehaviour
{
    // show a prompt in the inspector, with big text area
    [TextArea(3, 10)]
    public string Prompt;

    public TMP_Text TextMeshPro;

    async void OnMouseDown()
    {
        TextMeshPro.text = "...";
        var textResponse = await ClientLlama.singletonInstance.SendMessageToServer(Prompt);

        textResponse = Regex.Replace(textResponse, @"[^a-zA-Z0-9áéíóú ]", "");
        TextMeshPro.text = textResponse;
    }
}
