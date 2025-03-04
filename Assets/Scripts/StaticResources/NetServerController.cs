using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static JsonDataSaver;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

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





    public async Task<string> Register(string login, string email, string password)
    {
        var tcs = new TaskCompletionSource<string>();
        int requestId = GetRequestId();

        // �������� ������ � ������� SHA-256
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

        await SendRequest(requestId, "Register", $"{login} {email} {hashedPassword}");

        return await tcs.Task;
    }

    public string GetHashPass(string password)
    {
        return ComputeSha256Hash(password);
    }

    public async Task<string> Login(string login, string password)
    {
        var tcs = new TaskCompletionSource<string>();
        int requestId = GetRequestId();

        // �������� ������ � ������� SHA-256
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

        await SendRequest(requestId, "Login", $"{login} {hashedPassword}");

        return await tcs.Task;
    }

    public async Task<List<SoundData>> LoadSongs(string username, string password)
    {
        var tcs = new TaskCompletionSource<List<SoundData>>();
        int requestId = GetRequestId();

        // �������� ������
        string hashedPassword = ComputeSha256Hash(password);

        SetOnMessageReceivedListener(requestId, parts =>
        {
            if (parts.Count > 0)
            {
                // ��������� ����� �������
                List<SoundData> soundDataList = ParseSongs(parts);
                tcs.SetResult(soundDataList);
            }
            else
            {
                tcs.SetResult(new List<SoundData>()); // ���� ������ ���, ���������� ������ ������
            }
        });

        await SendRequest(requestId, "GetTopSongs", $"{username} {hashedPassword}");

        return await tcs.Task;
    }

    public async Task<SoundData> LoadSong(string username, string password, string muzPackName)
    {
        var tcs = new TaskCompletionSource<SoundData>();
        int requestId = GetRequestId();

        // �������� ������
        string hashedPassword = ComputeSha256Hash(password);

        SetOnMessageReceivedListener(requestId, parts =>
        {
            if (parts.Count > 0)
            {
                // ��������� ����� �������
                List<string> sd = new List<string>();
                sd.Add(parts[0]);
                SoundData soundDataList = ParseSongs(sd)[0];
                tcs.SetResult(soundDataList);
            }
            else
            {
                tcs.SetResult(null); // ���� ������ ���, ���������� ������ ������
            }
        });

        await SendRequest(requestId, "LoadSong", $"{username} {hashedPassword} {muzPackName}");

        return await tcs.Task;
    }

    public async Task<bool> PublishSong(string username, string password, MuzPackPreview muzPackPreview, string muzPackName)
    {
        var tcs = new TaskCompletionSource<bool>();
        int requestId = GetRequestId();

        // �������� ������
        string hashedPassword = ComputeSha256Hash(password);

        int bufferSize = Options.BufferSize;
        int maxParallelUploads = Options.MaxParallelUploads;
        int timeoutSeconds = Math.Max(1, bufferSize / 1024);
        string zipPath = MuzPackSaver.LoadMuzPackPath(muzPackName);

        if (!File.Exists(zipPath))
        {
            return false; // ���� �� ������
        }

        long fileSize = new FileInfo(zipPath).Length;
        UploadDownloadStatus.mustLoadedData = fileSize;
        UploadDownloadStatus.loadedData = 0;
        int totalParts = (int)Math.Ceiling((double)fileSize / bufferSize);

        SetOnMessageReceivedListener(requestId, parts =>
        {
            bool result = parts.Count > 0 && parts[0] == "true";
            Debug.Log($"Initial response received: {result}");
            tcs.SetResult(result);
        });

        await SendRequest(requestId, "SaveSong", $"{username} {hashedPassword} {totalParts} {muzPackPreview}");

        if (!await tcs.Task)
        {
            return false;
        }

        SemaphoreSlim semaphore = new SemaphoreSlim(maxParallelUploads);
        ConcurrentQueue<int> retryQueue = new ConcurrentQueue<int>();

        async Task<bool> SendPart(int partNumber, int totalParts, byte[] data)
        {
            string chunkBase64 = Convert.ToBase64String(data);
            string message = $"{muzPackPreview.Name.Replace(" ", "_")} {username} {partNumber} {totalParts} {chunkBase64}";

            var partTcs = new TaskCompletionSource<bool>();
            int partRequestId = GetRequestId();

            SetOnMessageReceivedListener(partRequestId, responseParts =>
            {
                bool partSuccess = responseParts.Count > 0 && responseParts[1] == partNumber.ToString() && responseParts[0] == "true";
                Debug.Log($"Part {partNumber} received: {partSuccess}");
                if (partSuccess)
                {
                    UploadDownloadStatus.loadedData += bufferSize; // ����������� uploadData
                }
                partTcs.TrySetResult(partSuccess);
            });

            Debug.Log($"Sending part {partNumber}/{totalParts}");
            await SendRequest(partRequestId, "UploadSongPart", message);

            var completedTask = await Task.WhenAny(partTcs.Task, Task.Delay(TimeSpan.FromSeconds(timeoutSeconds)));

            if (completedTask == partTcs.Task && await partTcs.Task)
            {
                return true;
            }
            else
            {
                Debug.Log($"Retrying part {partNumber}");
                retryQueue.Enqueue(partNumber);
                return false;
            }
        }

        using (FileStream fileStream = new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            List<Task> uploadTasks = new List<Task>();

            for (int i = 0; i < totalParts; i++)
            {
                byte[] buffer = new byte[Math.Min(bufferSize, (int)(fileSize - fileStream.Position))];
                await fileStream.ReadAsync(buffer, 0, buffer.Length);

                await semaphore.WaitAsync();
                int partNumber = i;

                uploadTasks.Add(Task.Run(async () =>
                {
                    await SendPart(partNumber, totalParts, buffer);
                    semaphore.Release();
                }));
            }

            await Task.WhenAll(uploadTasks);
        }

        while (!retryQueue.IsEmpty)
        {
            List<Task> retryTasks = new List<Task>();
            using (FileStream fileStream = new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                while (retryQueue.TryDequeue(out int retryPart))
                {
                    fileStream.Seek(retryPart * bufferSize, SeekOrigin.Begin);
                    byte[] buffer = new byte[Math.Min(bufferSize, (int)(fileSize - fileStream.Position))];
                    await fileStream.ReadAsync(buffer, 0, buffer.Length);

                    await semaphore.WaitAsync();
                    retryTasks.Add(Task.Run(async () =>
                    {
                        await SendPart(retryPart, totalParts, buffer);
                        semaphore.Release();
                    }));
                }
            }
            await Task.WhenAll(retryTasks);
        }

        Debug.Log("All parts successfully uploaded");
        return retryQueue.IsEmpty;
    }











    private string ComputeSha256Hash(string rawData)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2")); // ����������� � ����������������� �������������
            }
            return builder.ToString();
        }
    }

    /// <summary>
    /// ��������� ����� ������� � ��������� ������ SoundData.
    /// </summary>
    private List<SoundData> ParseSongs(List<string> serverResponse)
    {
        List<SoundData> soundDataList = new List<SoundData>();

        if (serverResponse.Count == 0 || serverResponse[0] != "true")
        {
            Debug.LogError("������ �������� ����� ��� ������ ������ ������ �����.");
            return soundDataList;
        }

        for (int i = 1; i < serverResponse.Count; i++) // ���������� "true"
        {
            string[] songParts = serverResponse[i].Split("__");
            if (songParts.Length != 4) continue; // ���������� ������������ ������

            string songName = songParts[0];
            int price = int.Parse(songParts[1]);
            int buyCount = int.Parse(songParts[2]);
            OwnerType ownType = ConvertToOwnerType(songParts[3]);

            // ������ ������ SoundData � null ��� ����������� � ����������
            SoundData soundData = new SoundData(
                songName,
                null, // �������� �����������
                null, // ��������� �����������
                new List<TimeValuePair>(), // ����� ��������� �������� ����� �����
                new OwnerData(songName, ownType),
                0
            );

            soundDataList.Add(soundData);
        }

        return soundDataList;
    }

    /// <summary>
    /// ������������ ��������� �������� ���� �������� � ������������ OwnerType
    /// </summary>
    private OwnerType ConvertToOwnerType(string ownTypeStr)
    {
        return ownTypeStr switch
        {
            "buyed" => OwnerType.buyed,
            "owner" => OwnerType.owner,
            _ => OwnerType.load
        };
    }
}
