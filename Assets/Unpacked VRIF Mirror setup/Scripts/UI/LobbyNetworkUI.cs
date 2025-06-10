using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utp;

namespace BNG
{
    public class PlayerInfo
    {
        public string playerName;
        public bool isReady;
    }
    
    public class LobbyNetworkUI : NetworkBehaviour
    {
        [Header("Network Manager")]
        [SerializeField] private RelayNetworkManager networkManager;
        
        [Header("UI References")]
        [SerializeField] private UITabSwitcher tabSwitcher;
        [SerializeField] private TMP_Text roomCodeText;
        [SerializeField] private Toggle readyToggle;
        [SerializeField] private GameObject startButton;
        [SerializeField] private TMP_Text readyCountText;
        [SerializeField] private GameObject playerUIPrefab;
        [SerializeField] private Transform playerListParent;
        
        private Dictionary<int, GameObject> playerUIObjects = new Dictionary<int, GameObject>();
        Dictionary<int, PlayerInfo> playerInfoDict = new Dictionary<int, PlayerInfo>();
        
        private int readyPlayers = 0;
        private int totalPlayers = 0;
        
        public void OnHost()
        {
            DisplayLobbyInfo();
            totalPlayers = 1;
        }
        
        public void OnConnect()
        {
            DisplayLobbyInfo();
        }
        
        /// <summary>
        /// Disconnects the client or host from the network session.
        /// </summary>
        public void OnDisconnectButton()
        {
            // int connId = sender.connectionId;
            // RemovePlayerUI(connId);
            if (isClient) networkManager.StopClient();
            if (isServer) networkManager.StopHost();
            
            tabSwitcher.ShowConnect();
        }
        
        [Command(requiresAuthority = false)]
        public void CmdPlayerReady(NetworkConnectionToClient sender = null)
        {
            int connId = sender.connectionId;

            if (!playerInfoDict.ContainsKey(connId))
            {
                playerInfoDict[connId] = new PlayerInfo
                {
                    playerName = $"Player {connId}", // Or pass in a real name
                    isReady = true
                };
            }
            else
            {
                playerInfoDict[connId].isReady = true;
            }

            UpdatePlayerListUI();
            UpdateReadyCounts();
        }
        
        [Command(requiresAuthority = false)]
        public void CmdPlayerNotReady(NetworkConnectionToClient sender = null)
        {
            int connId = sender.connectionId;

            if (playerInfoDict.ContainsKey(connId))
            {
                playerInfoDict[connId].isReady = false;

                UpdatePlayerListUI();
                UpdateReadyCounts();
            }
        }
        
        private void UpdateReadyCounts()
        {
            readyPlayers = 0;
            totalPlayers = playerInfoDict.Count;
            
            foreach (var player in playerInfoDict.Values)
            {
                if (player.isReady)
                {
                    readyPlayers++;
                }
            }
            
            RpcUpdateReadyCount(readyPlayers, totalPlayers);
            
            if (isServer) CheckStartCondition();
        }

        [ClientRpc]
        private void RpcUpdateReadyCount(int ready, int total)
        {
            readyPlayers = ready;
            totalPlayers = total;

            UpdatePlayerListUI();
        }
        
        [Command]
        public void CmdSetPlayerName(string name, NetworkConnectionToClient sender = null)
        {
            int connId = sender.connectionId;

            if (!playerInfoDict.ContainsKey(connId))
            {
                playerInfoDict[connId] = new PlayerInfo();
            }

            playerInfoDict[connId].playerName = name;
        }

        private void CheckStartCondition()
        {
            bool canStart = readyPlayers >= Mathf.CeilToInt(totalPlayers / 2f);
            startButton.SetActive(canStart);
        }
        
        public void OnToggle()
        {
            if (readyToggle.isOn)
            {
                CmdPlayerReady();
            }
            else
            {
                CmdPlayerNotReady();
            }
        }
        
        private void UpdatePlayerListUI()
        {
            foreach (var kvp in playerInfoDict)
            {
                int connId = kvp.Key;
                PlayerInfo info = kvp.Value;

                // Create UI if it doesn't exist yet
                if (!playerUIObjects.ContainsKey(connId))
                {
                    GameObject uiInstance = Instantiate(playerUIPrefab, playerListParent);
                    playerUIObjects[connId] = uiInstance;
                }

                // Update UI
                GameObject playerUI = playerUIObjects[connId];
                TMP_Text nameText = playerUI.GetComponentInChildren<TMP_Text>();
                Image bg = playerUI.GetComponent<Image>();

                nameText.text = info.playerName;
                bg.color = info.isReady ? Color.green : Color.white;
            }

            readyCountText.text = $"Ready: {readyPlayers}/{totalPlayers}";
        }
        
        private void RemovePlayerUI(int connId)
        {
            if (playerUIObjects.TryGetValue(connId, out GameObject ui))
            {
                Destroy(ui);
                playerUIObjects.Remove(connId);
            }

            playerInfoDict.Remove(connId);
            UpdateReadyCounts(); // Recalculate
        }

        public void OnStartGame()
        {
            
        }

        private void DisplayLobbyInfo()
        {
            roomCodeText.text = PlayerPrefs.GetString("RoomCode", "Unknown");
            CmdSetPlayerName(PlayerPrefs.GetString("PlayerName", $"Player {Random.Range(1000, 9999)}"));
            UpdatePlayerListUI();
        }
    }
}