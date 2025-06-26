using UnityEngine;
using Mirror;

namespace BNG
{
    // Script is on the player prefab to send player data to the BNG GameManager
    public class BNGLocalPlayerManager : NetworkBehaviour
    {
        private LocalPlayerData localPlayerData;

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
           
            localPlayerData = LocalPlayerData.Instance;
            if (localPlayerData != null)
            {
                string playerName = localPlayerData.playerName;
                //Debug.Log($"Local player name is: {playerName}");

                // Call command on server to add this player
                SendPlayerDataToServer(playerName);
            }
            else
            {
                Debug.LogError("LocalPlayerData not initialized.");
            }
        }

        private void SendPlayerDataToServer(string playerName)
        {      
            // Find the Game Manager and send the player data
            if (BNGGameManager.Instance != null)
            {
                BNGGameManager.Instance.CmdAddPlayer(playerName);
            }
        }
    }
}
