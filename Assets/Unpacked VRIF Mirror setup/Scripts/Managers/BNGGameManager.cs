using Mirror;

namespace BNG
{
    // Use a struct for cleaner code handling, doing this, you can have one dictionary key with several stats
    public struct PlayerGameStats
    {
        public string Name;
        public int KillCount;
        public int DeathCount;
    }

    // an example of controlling and tracking all players name, scores and progress in the game
    // this class can control everything in the game if needed as a single source of control, like timers etc.
    public class BNGGameManager : NetworkBehaviour
    { 
        public static BNGGameManager Instance;

        // Dictionary to hold player stats on the server
        public readonly SyncDictionary<string, PlayerGameStats> playerStats = new();

        private void Awake()
        {
            Instance = this;
        }

        // These commands are called from the client player to update the player stats when they change

        // Add the player to the manager on start of the game
        [Command(requiresAuthority = false)]
        public void CmdAddPlayer(string playerName, NetworkConnectionToClient sender = null)
        {
            string dictionaryKey = sender.ToString();
            playerStats.Add(dictionaryKey, new PlayerGameStats { Name = playerName, KillCount = 0, DeathCount = 0 });
        }

        // Update the death count of the player when they die
        [Command(requiresAuthority = false)]
        public void UpdateDeathCount(int playerDeathCount, NetworkConnectionToClient sender = null)
        {
            string dictionaryKey = sender.ToString();
            
            if (playerStats.ContainsKey(dictionaryKey))
            {
                PlayerGameStats stats = playerStats[dictionaryKey];
                stats.DeathCount = playerDeathCount;
                playerStats[dictionaryKey] = stats;
            }
        }

        // Update the kill count when a kill is made
        [Command(requiresAuthority = false)]
        public void UpdateKillCount(int playerKillCount, NetworkConnectionToClient sender = null)
        {
            string dictionaryKey = sender.ToString();

            if (playerStats.ContainsKey(dictionaryKey))
            {
                PlayerGameStats stats = playerStats[dictionaryKey];
                stats.KillCount = playerKillCount;
                playerStats[dictionaryKey] = stats;
            }
        }
    }
}
