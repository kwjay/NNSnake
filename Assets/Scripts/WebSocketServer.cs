using NativeWebSocket;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;
using static Utility;

public class WebSocketServer : MonoBehaviour
{
    [SerializeField] private AgentInterface _agent;
    [SerializeField] private uint _delay = 0;
    [SerializeField] private const int _maxRetries = 5;

    private WebSocket _websocket;
    private bool _waitingForAction = false;

    private void Start()
    {

        SetupConnection();
    }

    private void Update()
    {
        #if !UNITY_WEBGL || UNITY_EDITOR
            _websocket.DispatchMessageQueue();
        #endif
    }

    async private void SetupConnection()
    {
        _websocket = new WebSocket("ws://localhost:8080");
        _websocket.OnOpen += HandleOnOpen;
        _websocket.OnError += HandleOnError;
        _websocket.OnClose += HandleOnClose;
        _websocket.OnMessage += HandleOnMessageReceived;

        bool connected = false;
        int retries = 0;
        while (!connected && retries < _maxRetries)
        {
            try
            {
                Debug.Log("Attempting to connect to WebSocket...");
                await _websocket.Connect();
                connected = true;
            }
            catch (System.Exception ex)
            {
                retries++;
                Debug.LogWarning($"WebSocket connection failed (attempt {retries}). ({ex})");
                await Task.Delay(1000);
            }
        }

        if (!connected) Debug.LogError($"{nameof(SetupConnection)} Failed to connect to WebSocket after multiple attempts.");
    }

    private void HandleOnError(string error)
    {
        Debug.LogError("Error: " + error);
    }

    private async void HandleOnOpen()
    {
        Debug.Log("Connection open!");
        _agent.StartEpisode();
        await SendGameState();
    }

    private async void HandleOnMessageReceived(byte[] bytes)
    {
        if (!_waitingForAction) return;
        string json = Encoding.UTF8.GetString(bytes);
        Debug.Log($"Received from Python: {json}");
        try
        {
            int action = JsonUtility.FromJson<Utility.ActionMessage>(json).action;
            if (action < 0 || action > 2)
            {
                Debug.LogWarning($"Invalid action received: {action}");
                return;
            }
            _agent.Step(action);
            _waitingForAction = false;
            await Task.Delay((int)_delay);
            await SendGameState();
        }
        catch (System.Exception error)
        {
            _waitingForAction = false;
            Debug.LogError($"{nameof(HandleOnMessageReceived)} failed: {error.Message}");
        }
    }

    private void HandleOnClose(WebSocketCloseCode code)
    {
        Debug.Log("Connection closed!");
    }

    private async Task SendGameState()
    {
        if (_websocket.State != WebSocketState.Open)
        {
            Debug.LogWarning("WebSocket is not open. Cannot send game state.");
            return;
        }
        GameStateMessage msg = _agent.GetGameStateMessage();
        string json = JsonUtility.ToJson(msg);
        Debug.Log("Sending state to Python: " + json);
        await _websocket.SendText(json);
        _waitingForAction = true;
    }

    private async void OnApplicationQuit()
    {
        try
        {
            await _websocket.Close();
        }
        catch (System.Exception error)
        {
            Debug.LogWarning($"{nameof(OnApplicationQuit)} failed: {error.Message}");
        }
    }

}
