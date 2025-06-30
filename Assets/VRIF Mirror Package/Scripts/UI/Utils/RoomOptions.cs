using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utp;

namespace VRIF_Mirror_Package.Scripts.UI.Utils
{
    public class RoomOptions : MonoBehaviour
    {
        [SerializeField] private TMP_Text roomCodeText;
        private RelayNetworkManager _networkManager;
    
        private void Start()
        {
            _networkManager = FindFirstObjectByType<RelayNetworkManager>();
            if (_networkManager == null)
            {
                Debug.LogError("RelayNetworkManager not found in the scene.");
                return;
            }
        
            string roomCode = PlayerPrefs.GetString("RoomCode", "");
            if (string.IsNullOrEmpty(roomCode))
            {
                Debug.LogError("Room code is not set in PlayerPrefs.");
                return;
            }
        
            roomCodeText.text = $"Room Code:\n {roomCode}\n\n Press ≡ to Disable Menu";
        }

        public void OnDisconnectButton()
        {
            if (NetworkServer.active) _networkManager.StopHost();
            else if (NetworkClient.active) _networkManager.StopClient();
            else Debug.LogWarning("Neither server nor client is active.");
            ReworkedScreenFader sf = FindObjectOfType<ReworkedScreenFader>();
            
            sf.DoFadeIn(() => {
                SceneManager.LoadScene("MainMenu");
            }, Color.black);
        }
    }
}
