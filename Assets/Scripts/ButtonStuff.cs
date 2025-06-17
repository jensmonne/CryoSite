using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonStuff : MonoBehaviour
{
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void NewGame()
    {
        SceneManager.LoadScene("Level1");
    }
}
