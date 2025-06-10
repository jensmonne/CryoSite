using UnityEngine;
using Mirror;
using TMPro;

namespace BNG {
    public class PlayerNameDisplay : NetworkBehaviour
    {
        [SerializeField] private TMP_Text playerNameText;

        [SerializeField] private Canvas PlayerInfoCanvas;

        [Tooltip("If true this transform will always look at the Camera in Update")]
        public bool LookAtCamera = true;

        // Cache the camera transform if we want to look at it in Update
        Transform camTransform;

        //LocalPlayerData localPlayerData;

        private string localPlayerData;
        string settingsPlayerName;

        [SyncVar(hook = nameof(SetNetworkPlayerName))]
        public string networkPlayerName;

        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
            if (!isOwned) return;
            // Retrieve our player name from local data
            //localPlayerData = LocalPlayerData.Instance;
                
            localPlayerData = PlayerPrefs.GetString("PlayerName", "Unknown");

            // disable the player label over the local player
            PlayerInfoCanvas.enabled = false;
        }

        private void Update() {
            CheckNameUpdate();

            if (LookAtCamera) {
                UpdateCameraLook();
            }
        }

        public void CheckNameUpdate() {
            if (isOwned && localPlayerData != null) {
                // If name has changed in local data, send to network so we can update our label
                /*if (settingsPlayerName != localPlayerData.PlayerName) {

                    settingsPlayerName = localPlayerData.PlayerName;

                    CmdSetNetworkPlayerName(settingsPlayerName);
                }*/
                
                if (settingsPlayerName != localPlayerData) {
                    settingsPlayerName = localPlayerData;

                    CmdSetNetworkPlayerName(settingsPlayerName);
                }
            }
        }

        [Command]
        public void CmdSetNetworkPlayerName(string _npName) {
            networkPlayerName = _npName;
        }

        private void SetNetworkPlayerName(string oldName, string newName) {
            playerNameText.text = networkPlayerName;
        }

        public virtual void AssignCameraTransform() {
            if (_camera != null) {
                camTransform = _camera.transform;
            }
        }

        public virtual void UpdateCameraLook() {
            if (camTransform == null) {
                AssignCameraTransform();
            }

            if (PlayerInfoCanvas != null && camTransform != null) {
                PlayerInfoCanvas.transform.LookAt(PlayerInfoCanvas.transform.position + camTransform.transform.rotation * Vector3.forward, Vector3.up);
                // Alternatively follow camera axis :
                // PlayerInfoCanvas.transform.LookAt(PlayerInfoCanvas.transform.position + camTransform.transform.rotation * Vector3.forward, camTransform.transform.rotation * Vector3.up);
            }
        }
    }
}
