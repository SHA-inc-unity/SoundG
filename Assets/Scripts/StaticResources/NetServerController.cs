using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class NetServerController : MonoBehaviour
{
    private static readonly string serverAddress = "ws://95.165.27.159:17925/";
    private static ClientWebSocket webSocket;
    private static int requestId = 0;
    private static Dictionary<int, Action<List<string>>> listeners = new();
    private static bool isReconnecting = false;
    private const int reconnectDelaySeconds = 5;
    private CancellationTokenSource cts = new();

    private static NetServerController _instance;

    public static NetServerController Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("NetServerController");
                _instance = obj.AddComponent<NetServerController>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    private async void Start()
    {
        await Connect();
    }

    private async Task Connect()
    {
        if (webSocket != null)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }

        webSocket = new ClientWebSocket();

        try
        {
            await webSocket.ConnectAsync(new Uri(serverAddress), CancellationToken.None);
            Debug.Log("Connected to WebSocket server.");
            _ = ReceiveMessages();
        }
        catch (Exception e)
        {
            Debug.LogError($"WebSocket connection error: {e.Message}");
            _ = AttemptReconnect();
        }
    }

    private async Task AttemptReconnect()
    {
        if (isReconnecting) return;
        isReconnecting = true;

        Debug.Log("Attempting to reconnect...");
        await Task.Delay(reconnectDelaySeconds * 1000);
        await Connect();
        isReconnecting = false;
    }

    private async Task ReceiveMessages()
    {
        var buffer = new byte[1024];

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Debug.Log("WebSocket closed by server.");
                    await AttemptReconnect();
                    return;
                }

                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                OnTextReceived(message);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"WebSocket receive error: {e.Message}");
            await AttemptReconnect();
        }
    }

    private void OnTextReceived(string message)
    {
        string[] parts = message.Split(' ');
        if (parts.Length == 0 || !int.TryParse(parts[0], out int id))
        {
            Debug.Log($"Invalid message format: {message}");
            return;
        }

        if (listeners.TryGetValue(id, out var listener))
        {
            listener(new List<string>(parts[1..]));
            listeners.Remove(id);
        }
    }

    public void SetOnMessageReceivedListener(int id, Action<List<string>> listener)
    {
        listeners[id] = listener;
    }

    public async Task SendRequest(int id, string requestWord, string message)
    {
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            string fullMessage = $"/sql {requestWord} {id} {message}";
            await webSocket.SendAsync(
                new ArraySegment<byte>(Encoding.UTF8.GetBytes(fullMessage)),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );
        }
        else
        {
            Debug.Log("WebSocket is not connected. Reconnecting...");
            await Connect();
        }
    }

    public async Task SendUnregisteredRequest(string message)
    {
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            await webSocket.SendAsync(
                new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );
        }
        else
        {
            Debug.Log("WebSocket is not connected. Reconnecting...");
            await Connect();
        }
    }

    public int GetRequestId()
    {
        requestId++;
        if (requestId >= 1000000)
        {
            requestId = 0;
        }
        return requestId;
    }

    private async void OnDestroy()
    {
        if (webSocket != null)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Destroying", CancellationToken.None);
        }
    }





    public async Task<string> Register(string login, string password)
    {
        var tcs = new TaskCompletionSource<string>();
        int requestId = GetRequestId();

        // Хешируем пароль с помощью SHA-256
        string hashedPassword = ComputeSha256Hash(password);

        SetOnMessageReceivedListener(requestId, parts =>
        {
            if (parts.Count > 0)
            {
                tcs.SetResult(parts[0]);
            }
            else
            {
                tcs.SetResult(null);
            }
        });

        await SendRequest(requestId, "SendMessage", $"{login} {hashedPassword}");

        return await tcs.Task;
    }

    public async Task<string> Login(string login, string password)
    {
        var tcs = new TaskCompletionSource<string>();
        int requestId = GetRequestId();

        // Хешируем пароль с помощью SHA-256
        string hashedPassword = ComputeSha256Hash(password);

        SetOnMessageReceivedListener(requestId, parts =>
        {
            if (parts.Count > 0)
            {
                tcs.SetResult(parts[0]);
            }
            else
            {
                tcs.SetResult(null);
            }
        });

        await SendRequest(requestId, "SendMessage", $"{login} {hashedPassword}");

        return await tcs.Task;
    }

    private string ComputeSha256Hash(string rawData)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2")); // Преобразуем в шестнадцатеричное представление
            }
            return builder.ToString();
        }
    }
}
