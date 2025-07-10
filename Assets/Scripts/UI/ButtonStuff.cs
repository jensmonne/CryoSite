using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonStuff : MonoBehaviour
{
    public void MainMenu()
    {
        if (NetworkServer.active && NetworkClient.isConnected) NetworkManager.singleton.StopHost();
        else if (NetworkClient.isConnected) NetworkManager.singleton.StopClient();
        else if (NetworkServer.active) NetworkManager.singleton.StopServer();
        
        SceneManager.LoadScene("MainMenu");
    }
    
    public void ChangeScene()
    {
        if (NetworkServer.active)
        {
            NetworkManager.singleton.ServerChangeScene("MapOnline");
        }

        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            SceneManager.LoadScene("MapOnline");
        }
    }

    public void SceneChange()
    {
        SceneManager.LoadScene("Map");
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}
