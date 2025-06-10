using UnityEngine;

// This class holds all the player settings and does not get destroyed on scene change
// The script saves data to PlayerPrefs(change save system to your preference) and is recalled on restart
namespace BNG {
    public class LocalPlayerData : MonoBehaviour {
        public static LocalPlayerData Instance {
            get {
                if (_instance == null) {
                    _instance = FindObjectOfType<LocalPlayerData>();
                    if (_instance == null) {
                        _instance = new GameObject("LocalPlayerData").AddComponent<LocalPlayerData>();
                    }
                }
                return _instance;
            }
        }
        private static LocalPlayerData _instance;

        // Any local player data we may want to store for later
        public string playerName;
        public int playerPrefabIndex = 0;
        public int bodyTextureIndex = 0;
        public int propTextureIndex = 0;

        private void Awake() {
            // Setup Singleton so only one object exists at a time
            if (_instance != null && _instance != this) {
                Destroy(this);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        // Set player prefab selection index from menu
        // These are called from Local Avatar Settings on the Player Controller
        public void SetPlayerPrefabIndex(int prefabIndex)
        {
            playerPrefabIndex = prefabIndex;
        }

        public void SetBodyTextureIndex(int preBodyTextIndex)
        {
            bodyTextureIndex = preBodyTextIndex;
        }

        public void SetPropTextureIndex(int prePropTextIndex)
        {
            propTextureIndex = prePropTextIndex;
        }
    }
}
