using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ClientLlama : MonoBehaviour
{
    private ClientWebSocket clientWebSocket;

    [SerializeField] 
    private string serverAddress = "ws://localhost:8765";

    public static ClientLlama singletonInstance;

    async void Start()
    {
        if (singletonInstance != null)
        {
            Destroy(this);
            return;
        }

        singletonInstance = this;

        clientWebSocket = new ClientWebSocket();
        await clientWebSocket.ConnectAsync(new Uri(serverAddress), CancellationToken.None);
        print("Connected to WebSocket server.");
    }


    private async void OnApplicationQuit()
    {
        if (clientWebSocket.State == WebSocketState.Open)
        {
            await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }
    }

    public async Task<string> SendMessageToServer(string message)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        await clientWebSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
        print($"Message sent: {message}");

        byte[] buffer = new byte[1024];
        var receiveResult = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        if (receiveResult.MessageType == WebSocketMessageType.Text)
        {
            string receivedMessage = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
            return receivedMessage;
        }
        return "No response received";
    }


    [ContextMenu("Send Message")]
    public async void SendMessage()
    {
        var message = "the capital of France is";
        var response = await SendMessageToServer(message);
        print($"¡¡¡¡¡¡ Response: {response}");
    }
}
