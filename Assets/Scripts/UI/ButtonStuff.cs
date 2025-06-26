using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonStuff : NetworkBehaviour
{
    public void MainMenu()
    {
        if (isClient) NetworkManager.singleton.StopClient();

        if (isServer) NetworkManager.singleton.StopHost();
        
        SceneManager.LoadScene("MainMenu");
    }
    
    [Server]
    public void ChangeScene()
    {
        // Change the scene for all clients
        //  ServerChangeScene(sceneToLoad);
        NetworkManager.singleton.ServerChangeScene("Level1");
    }

    public void SceneChange()
    {
        SceneManager.LoadScene("Level1Offline");
    }
    
    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
