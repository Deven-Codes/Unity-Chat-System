using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace LoginSystem
{
    public class ChatPanel : MonoBehaviour
    {
        // Chat Text GUI
        public TextMeshProUGUI chatText;
        
        // Start is called before the first frame update
        void Start()
        {
            // Set text to empty string
            chatText.text = "";
        }

        public void SendMessageToChat(string text)
        {
            // Append new messages
            chatText.text += text+ "\n";
        }
    }
}