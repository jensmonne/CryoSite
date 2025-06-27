using UnityEngine;
using UnityEngine.SceneManagement;
using VRIF_Mirror_Package.Scripts.UI.Utils;

public class EndingStuff : MonoBehaviour
{
    private ReworkedScreenFader fader;
    
    private void Start()
    {
        fader = FindAnyObjectByType<ReworkedScreenFader>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!GameManager.IsBombActive) return;

        GameManager.Instance.CurrentState = GameManager.GameState.GameWon;
        
        fader.DoFadeIn(() => {
            SceneManager.LoadScene("DarkBox");
        }, Color.black);
    }
}
