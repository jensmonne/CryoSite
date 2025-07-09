using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonStuff : MonoBehaviour
{
    public void MainMenu()
    {
        if (NetworkServer.active && NetworkClient.isConnected) NetworkManager.singleton.StopClient();

        if (NetworkClient.isConnected) NetworkManager.singleton.StopHost();
        
        SceneManager.LoadScene("MainMenu");
    }
    
    [Server]
    public void ChangeScene()
    {
        NetworkManager.singleton.ServerChangeScene("MapOnline");
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
