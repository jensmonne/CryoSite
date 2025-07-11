using Mirror;

namespace VRIF_Mirror_Package.Scripts.UI.Utils
{
    public class QuitGameUI : NetworkBehaviour
    {
        public UnityEngine.UI.Button quitButton;

        private void Start()
        {
            quitButton.onClick.AddListener(QuitGame);
        }
        
        private void QuitGame()
        {
            if (isClient) NetworkManager.singleton.StopClient();

            if (isServer) NetworkManager.singleton.StopHost();
        }
    }
}
