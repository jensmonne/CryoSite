using UnityEngine;

/// <summary>
/// This script has to start asap when the player loads into the darkbox scene. It handles the win/lose screen, you'll find out why yourself...
/// </summary>
public class DarkBoxStart : MonoBehaviour
{
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    
    private void Start()
    {
        if (GameManager.Instance.CurrentState == GameManager.GameState.GameWon) GetWinScreenStuff();
        else GetLoseScreenStuff();
    }

    private void GetWinScreenStuff()
    {
        winScreen.SetActive(true);
    }

    private void GetLoseScreenStuff()
    {
        loseScreen.SetActive(true);
    }
}
