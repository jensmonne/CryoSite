using System;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Utp;

namespace BNG {
    public class SimpleNetworkUI : NetworkBehaviour
    {
        private ScreenFader screenFader;
        
        [Header("Network Manager")]
        [SerializeField] private RelayNetworkManager networkManager;

        [Header("Input Fields")]
        [SerializeField] private InputField playerNameInput;
        [SerializeField] private InputField roomCodeInput;
        
        [SerializeField] private TMP_Text displayText;
        
        [SerializeField] private SceneLoader sceneLoader;

        private bool clientConnected;
        private string relayJoinCode;
        private bool isAuthenticated = false;
        private bool isAuthenticating = false;
        
        private void Start()
        {
            screenFader = FindObjectOfType<ScreenFader>();
        }

        private async Task EnsureAuthentication()
        {
            if (isAuthenticated || isAuthenticating) return;

            isAuthenticating = true;
            try
            {
                if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
                {
                    await UnityServices.InitializeAsync();
                    Debug.Log("Unity Services Initialized");
                }

                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    Debug.Log("Signed into Unity Services! Player ID: " + AuthenticationService.Instance.PlayerId);
                }

                isAuthenticated = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Unity Services authentication failed: {e.Message}");
            }
            finally
            {
                isAuthenticating = false;
            }
        }

        public override void OnStartServer()
        {
            Debug.Log("OnStartServer");
            displayText.text = "Server started.\n";
        }

        public override void OnStartClient()
        {
            Debug.Log("OnStartClient");
            clientConnected = true;
            displayText.text += "Client started.\n";
        }

        public async void OnHostButton()
        {
            displayText.text = "Attempting to host server... \n";
            
            string playerName = playerNameInput.text;
            if (string.IsNullOrEmpty(playerName))
            {
                displayText.text = "Please enter your player name.\n";
                return;
            }
            
            PlayerPrefs.SetString("PlayerName", playerName);
            
            await EnsureAuthentication();
            if (!isAuthenticated)
            {
                Debug.Log("Not authenticated");
                return;
            }
            
            screenFader.DoFadeIn();
            
            networkManager.StartRelayHost(4, () =>
            {
                displayText.text += "Hosting server...\n";
            });
        }

        public async void OnConnectButton()
        {
            displayText.text = "Connecting to Relay...\n";

            string joinCode = roomCodeInput.text.Trim();
            
            if (string.IsNullOrEmpty(joinCode) || joinCode.Length != 6)
            {
                displayText.text = "Please enter a valid join code.\n";
                return;
            }
            
            PlayerPrefs.SetString("RoomCode", joinCode);
            
            string playerName = playerNameInput.text;
            
            if (string.IsNullOrEmpty(playerName))
            {
                displayText.text = "Please enter your player name.\n";
                return;
            }
            
            PlayerPrefs.SetString("PlayerName", playerName);

            await EnsureAuthentication();
            if (!isAuthenticated)
            {
                Debug.Log("Not authenticated");
                return;
            }

            screenFader.DoFadeIn();

            networkManager.JoinRelayServer(joinCode);
        }

        public void OnDisconnectButton()
        {
            if (isClient) networkManager.StopClient();
            if (isServer) networkManager.StopHost();
        }

        public void OnOfflineButton()
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(2));
            sceneLoader.LoadScene(sceneName);
        }

        public override void OnStopClient()
        {
            Debug.Log("OnStopClient");
            displayText.text += "Client disconnected.\n";
            clientConnected = false;
        }
    }
}
