using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonStuff : MonoBehaviour
{
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    [Server]
    public void ChangeScene()
    {
        // Change the scene for all clients
        //  ServerChangeScene(sceneToLoad);
        NetworkManager.singleton.ServerChangeScene("Level1");
    }
    
    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
