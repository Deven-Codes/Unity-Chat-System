using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Threading;


namespace LoginSystem
{

    public class Login : MonoBehaviour
    {
        
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private Button signInButton;
        [SerializeField] private Button createButton;
        [SerializeField] private TMP_InputField usernameInputField;
        [SerializeField] private TMP_InputField passwordInputField;
        [SerializeField] private GameObject loginUI;
        [SerializeField] private GameObject chatBox;
        [SerializeField] private GameObject websocketConnection;

        private string authenticationEndpoint = "http://127.0.0.1:5000/account";

        // Username Property of the logged in account 
        private string _username;
        public string Username
        {
            get
            {
                return _username;
            }

            private set
            {
                _username = value;
            }
        }




        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            loginUI.SetActive(true);
            chatBox.SetActive(false);
            websocketConnection.SetActive(false);
            headerText.text = "Enter your credentials";
        }

        /// <summary>
        /// Called when user presses Login Button.
        /// </summary>
        public void OnLoginClick()
        {
            headerText.text = "Signing in...";
            ActivateButton(false);
            StartCoroutine(TryLogin());
        }

        /// <summary>
        /// Called when user pressed Create Button
        /// </summary>
        public void OnCreateClick()
        {
            headerText.text = "Creating in...";
            ActivateButton(false);
            StartCoroutine(TryCreate());
        }

        /// <summary>
        /// Handles user login
        /// </summary>
        /// <returns>yield return null</returns>
        private IEnumerator TryLogin()
        {
            // Get input from the username input field
            string username = usernameInputField.text;

            // Get password from the password field.
            string password = passwordInputField.text;

            // Check if the credentials are valid
            if (!ValidCredentials(username, password))
            {
                // break coroutine function
                yield break;
            }

            // Create request form for login request
            WWWForm form = new WWWForm();
            form.AddField("rUsername", username); // add username field
            form.AddField("rPassword", password); // add password field

            // Send web request to the server using Post method.
            UnityWebRequest request = UnityWebRequest
                                        .Post(authenticationEndpoint + "/login", form);

            // Get request handler 
            var handler = request.SendWebRequest();

            // Start timer of 10 seconds to get login response 
            float start = 0.0f;

            // while handler is not done 
            // kepps timer running for 10 seconds (max)
            while (!handler.isDone) 
            {
                start += Time.deltaTime;

                if (start > 10.0f)
                {
                    // breaks after 10 seconds
                    break;
                }

                yield return null;
            }

            // Handler is done
            // If server responded successfully
            if (request.result == UnityWebRequest.Result.Success)
            {
                // Loads data into JSON
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

                // Checks response code
                switch (response.code)
                {
                    // Login Successfully
                    case 0:
                        // Set login and create button to false
                        ActivateButton(false);

                        // Change header text 
                        headerText.text = "Welcome";

                        // Set Username to logged in username
                        Username = usernameInputField.text;

                        // Conncet to websocket server 
                        ConnectToWSServer();
                        break;

                    //  Login fails
                    case 1:
                        headerText.text = "Invalid credentials";
                        ActivateButton(true);
                        break;

                    // Default fail (Security measure)
                    default:
                        headerText.text = "Error!!";
                        ActivateButton(false);
                        break;

                }

            }

            // Server didn't respond to the request 
            else
            {
                headerText.text = "Error connecting to the server";
                ActivateButton(true);
            }

            yield return null;
        }


        private IEnumerator TryCreate()
        {
            // Get input from the username input field
            string username = usernameInputField.text;

            // Get input from the password input field
            string password = passwordInputField.text;

            // Check if the credentials are valid
            if (!ValidCredentials(username, password))
            {
                yield break;
            }

            // Create request form for login request
            WWWForm form = new WWWForm();
            form.AddField("rUsername", username); // add username field
            form.AddField("rPassword", password); // add password field

            // Send create account request to the server using Post method.
            UnityWebRequest request = UnityWebRequest
                                        .Post(authenticationEndpoint + "/create", form);

            // Get request handler 
            var handler = request.SendWebRequest();

            // Start timer of 10 seconds to get login response 
            float start = 0.0f;

            // while handler is not done 
            // kepps timer running for 10 seconds (max)
            while (!handler.isDone)
            {
                start += Time.deltaTime;

                if (start > 10.0f)
                {
                    // breaks after 10 seconds
                    break;
                }

                yield return null;
            }

            // Handler is done
            // If server responded successfully
            if (request.result == UnityWebRequest.Result.Success)
            {
                // Loads data into JSON
                CreateResponse response = JsonUtility.FromJson<CreateResponse>(request.downloadHandler.text);

                // Check repsonse code
                switch (response.code)
                {
                    // Account Created Succefully
                    case 0:
                        headerText.text = "Account has been created";
                        break;

                    // Invalid Credentials
                    case 1:
                        headerText.text = "Invalid Credentials";
                        break;
                    
                    // Username already in use
                    case 2:
                        headerText.text = "Username already taken";
                        break;

                    // Default Error
                    default:
                        headerText.text = "Error!!";
                        break;
                }
            }
            // Server didn't respond to the request 
            else
            {
                headerText.text = "Error connecting to the server";
            }

            ActivateButton(true);
            yield return null;
        }

        // Check if Credentials are valid
        private bool ValidCredentials(string username, string password)
        {
            if (username.Length < 3 || username.Length > 24)
            {
                headerText.text = "Invalid Username";
                ActivateButton(true);
                return false;
            }

            if (password.Length < 3 || password.Length > 24)
            {
                headerText.text = "Invalid Password";
                ActivateButton(true);
                return false;
            }
            return true;
        }

        // Toggle Login and Create button
        private void ActivateButton(bool toggle)
        {
            if(signInButton != null)
                signInButton.interactable = toggle;
            if(createButton != null)
                createButton.interactable = toggle;
        }


        // Connect to WebSocket Server
        private void ConnectToWSServer()
        {
            loginUI.SetActive(false);
            chatBox.SetActive(true);
            websocketConnection.SetActive(true);
        }

        // Reset Connections
        public void ResetConnection(string data)
        {
            Debug.Log($"Closed: {data}");
            chatBox.SetActive(false);
            loginUI.SetActive(true);
            headerText.text = "Connection Closed";
            usernameInputField.text = "";
            passwordInputField.text = "";
            ActivateButton(true);
        }
    }  
}