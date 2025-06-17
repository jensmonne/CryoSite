using BNG;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingStuff : MonoBehaviour
{
    private ScreenFader fader;
    
    private void Start()
    {
        fader = FindObjectOfType<ScreenFader>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!GameManager.IsBombActive) return;

        GameManager.Instance.CurrentState = GameManager.GameState.GameWon;
        
        fader.DoFadeIn(() => {
            SceneManager.LoadScene("DarkBox");
        });
    }
}
