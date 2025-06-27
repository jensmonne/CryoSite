using UnityEngine;
using UnityEngine.SceneManagement;
using VRIF_Mirror_Package.Scripts.UI.Utils;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxPlayerHealth = 100f;

    [SerializeField] private float playerHealth;

    private ReworkedScreenFader fader;
    
    private void Start()
    {
        playerHealth = maxPlayerHealth;
        fader = FindAnyObjectByType<ReworkedScreenFader>();
    }

    public void TakeDamage(float amount)
    {
        playerHealth = Mathf.Clamp(playerHealth - amount, 0, maxPlayerHealth);

        if (playerHealth <= 0) Death();
    }

    public void Heal(float amount)
    {
        playerHealth = Mathf.Clamp(playerHealth + amount, 0, maxPlayerHealth);
    }

    private void Death()
    {
        GameManager.Instance.CurrentState = GameManager.GameState.GameOver;
        
        fader.DoFadeIn(() => {
            SceneManager.LoadScene("DarkBox");
        }, Color.red);
    }
}
