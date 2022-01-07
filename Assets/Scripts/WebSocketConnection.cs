using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using LoginSystem;
using TMPro;


namespace LoginSystem
{

    public class WebSocketConnection : MonoBehaviour
    {

        public string username;
        private string webSocketEndpoint = "ws://127.0.0.1:5050";
        private WebSocket websocket;
        [SerializeField] private Login login;
        [SerializeField] private ChatPanel chatPanel;
        [SerializeField] private TMP_InputField messageInputField;

        // Start is called before the first frame update
        async void Start()
        {
            // Set username
            username = login.Username;

            // Create websocket client
            websocket = new WebSocket(webSocketEndpoint);

            // WebSocket open connection with server
            websocket.OnOpen += () =>
            {
                Debug.Log("Connection open!");

                // Send Message to server 
                SendWebSocketMessage($"** {username} connected **");

            };

            // WebSocket error connection 
            websocket.OnError += (e) =>
            {
                Debug.Log("Error! " + e);
            };

            // WebSocket close 
            websocket.OnClose += (e) =>
            {
                Debug.Log("Connection closed!");
                login.ResetConnection(e.ToString());
            };

            // On receiving message from the server
            websocket.OnMessage += (bytes) =>
            {
                Debug.Log("OnMessage!");

                // getting the message as byte array
                // Converting byte array to string
                var message = System.Text.Encoding.UTF8.GetString(bytes);
                Debug.Log("OnMessage! " + message);

                // Sending string message to chatbox
                chatPanel.SendMessageToChat(message);
            };

            // Wait for websocket to connect
            await websocket.Connect();
        }

        void Update()
        {
            #if !UNITY_WEBGL || UNITY_EDITOR
                websocket.DispatchMessageQueue();
            #endif

            // When pressed enter send message to server
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessage();
            }
        }

        // On send button click send message to the server 
        public void OnSendButtonClick()
        {
            
            SendMessage();
            
        }

        // Send message to server
        private void SendMessage()
        {
            if (messageInputField.text.Length > 0)
            {
                string text = $"{username}: {messageInputField.text}";
                // Paasing message as a string
                SendWebSocketMessage(text);
            }
        }

        // Send data to server (asyn)
        public async void SendWebSocketMessage(string text)
        {
            // Check if websocket is stil open
            if (websocket.State == WebSocketState.Open)
            {
                // Sanity Check
                if (websocket != null)
                {
                    // Convert string to byte array
                    var bytes = System.Text.Encoding.UTF8.GetBytes(text);

                    // Send byte array to server 
                    await websocket.Send(bytes);

                    // Resets messageInput 
                    messageInputField.text = "";
                }
            }
        }

        // On Application quite close connection
        private async void OnApplicationQuit()
        {
            await websocket.Close();
        }

    }
}