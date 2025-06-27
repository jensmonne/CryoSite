using Mirror;
using UnityEngine;

namespace VRIF_Mirror_Package.Scripts.SceneLoading
{
    public class ServerChangeScene : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Button changeSceneButton;
        
#if UNITY_EDITOR
        [Tooltip("The scene that loads when you press the button.")]
        [SerializeField] private UnityEditor.SceneAsset sceneToLoad;
#endif
        
        [Tooltip("Name of the scene to load (auto-filled from sceneToLoad).")]
        [SerializeField, HideInInspector] private string sceneName;

        private void Start()
        {
            // Ensure the button is set up to call ChangeScene on click
            if (changeSceneButton != null)
                changeSceneButton.onClick.AddListener(OnChangeSceneButtonClicked);
            else
                Debug.LogWarning("Change Scene Button is not assigned.");
        }

        private void OnChangeSceneButtonClicked()
        {
            // Only the server should change the scene
            if (NetworkServer.active) ChangeScene();
            else Debug.LogWarning("Only the server can change the scene!");
        }

        [Server]
        private void ChangeScene()
        {
            if (!string.IsNullOrEmpty(sceneName))
                NetworkManager.singleton.ServerChangeScene(sceneName);
            else Debug.LogError("Scene name is not set. Assign a scene in the inspector.");
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (sceneToLoad != null)
            {
                string path = UnityEditor.AssetDatabase.GetAssetPath(sceneToLoad);
                sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
            }
            else sceneName = string.Empty;
        }
#endif
    }
}
