using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

namespace VRIF_Mirror_Package.Scripts.UI
{
    public class ReworkedLobbyNetworkUI : MonoBehaviour
    {
        public Transform playerListParent;
        public GameObject playerUIPrefab;
        public TMP_Text readyCountText;
        public GameObject startButton;

        private List<NetworkedPlayer> players = new();
        private Dictionary<NetworkedPlayer, GameObject> playerUIs = new();

        public void AddPlayer(NetworkedPlayer player)
        {
            if (!players.Contains(player))
            {
                players.Add(player);
                CreateOrUpdateUI(player);
                UpdateReadyCounts();
            }
        }

        public void RemovePlayer(NetworkedPlayer player)
        {
            if (players.Contains(player))
            {
                players.Remove(player);
                if (playerUIs.TryGetValue(player, out var ui)) Destroy(ui);
                playerUIs.Remove(player);
                UpdateReadyCounts();
            }
        }

        public void UpdatePlayerUI(NetworkedPlayer player)
        {
            CreateOrUpdateUI(player);
            UpdateReadyCounts();
        }

        private void CreateOrUpdateUI(NetworkedPlayer player)
        {
            if (!playerUIs.TryGetValue(player, out var ui))
            {
                ui = Instantiate(playerUIPrefab, playerListParent);
                playerUIs[player] = ui;
            }

            var text = ui.GetComponent<PlayerUIPrefab>().nameText;
            text.text = player.playerName;
            text.color = player.isReady ? Color.green : Color.white;
        }

        private void UpdateReadyCounts()
        {
            int ready = 0;
            foreach (var p in players)
            {
                if (p.isReady) ready++;
            }
            readyCountText.text = $"Ready: {ready}/{players.Count}";
            startButton.SetActive(NetworkServer.active && ready >= Mathf.CeilToInt(players.Count / 2f));
        }
    }
}