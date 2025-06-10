using UnityEngine;

namespace BNG
{
    // Script to save and load data from PlayerPrefs
    public class SaveLoadData : MonoBehaviour
    {
        public static SaveLoadData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<SaveLoadData>();
                    if (_instance == null)
                    {
                        _instance = new GameObject("SaveLoadData").AddComponent<SaveLoadData>();
                    }
                }
                return _instance;
            }
        }
        private static SaveLoadData _instance;

        LocalPlayerData localPlayerData;

        [Header("Player Name Input field on the Menu")]
        public UnityEngine.UI.InputField playerNameInputField;
   
        private void Start()
        {
            localPlayerData = LocalPlayerData.Instance;
            LoadFromPrefs();
        }

        private void LoadFromPrefs()
        {   
            // Load saved player name and load it to the menu input and local player data
            string playerName = (string)LoadPlayerPref("PlayerName", "Unknown");
            if (playerName != "Unknown")
            {
                localPlayerData.playerName = playerName;
                playerNameInputField.text = playerName;
            }
            int playerPrefab = (int)LoadPlayerPref("PrefabIndex", 0);
            localPlayerData.playerPrefabIndex = playerPrefab;
            int playerBodyTexture = (int)LoadPlayerPref("BodyTextureIndex", 0);
            localPlayerData.bodyTextureIndex = playerBodyTexture;
            int playerPropTexture = (int)LoadPlayerPref("PropTextureIndex", 0);
            localPlayerData.propTextureIndex = playerPropTexture;
        }

        // Function called from LoadFromPrefs
        public object LoadPlayerPref(string key, object defaultValue)
        {
            return defaultValue switch
            {
                int i => PlayerPrefs.GetInt(key, i),
                float f => PlayerPrefs.GetFloat(key, f),
                string s => PlayerPrefs.GetString(key, s),
                _ => LogAndReturnNull()
            };
        }
        
        private object LogAndReturnNull()
        {
            Debug.LogError("Unsupported type for PlayerPrefs");
            return null;
        }

        // Call this function to save a pref
        public void SavePlayerPref(string key, object value)
        {
            switch (value)
            {
                case int i:
                    PlayerPrefs.SetInt(key, i);
                    break;
                case float f:
                    PlayerPrefs.SetFloat(key, f);
                    break;
                case string s:
                    PlayerPrefs.SetString(key, s);
                    break;
                default:
                    Debug.LogError("Unsupported type for PlayerPrefs");
                    break;
            }

            PlayerPrefs.Save(); 
        }
    }
}
