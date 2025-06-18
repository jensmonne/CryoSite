using BNG;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxPlayerHealth = 100f;

    [SerializeField] private float playerHealth;

    private ScreenFader fader;
    
    private void Start()
    {
        playerHealth = maxPlayerHealth;
        fader = FindObjectOfType<ScreenFader>();
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
