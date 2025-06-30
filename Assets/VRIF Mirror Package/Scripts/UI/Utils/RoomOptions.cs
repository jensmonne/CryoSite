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
        private RelayNetworkManager networkManager;
    
        private void Start()
        {
            networkManager = FindFirstObjectByType<RelayNetworkManager>();
            if (networkManager == null)
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
            if (NetworkServer.active) networkManager.StopHost();
            else if (NetworkClient.active) networkManager.StopClient();
            else Debug.LogWarning("Neither server nor client is active.");
            ReworkedScreenFader sf = FindObjectOfType<ReworkedScreenFader>();
            
            sf.DoFadeIn(() => {
                SceneManager.LoadScene("MainMenu");
            }, Color.black);
        }
    }
}
