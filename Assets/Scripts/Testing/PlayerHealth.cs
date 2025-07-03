using UnityEngine;
using UnityEngine.SceneManagement;
using VRIF_Mirror_Package.Scripts.UI.Utils;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxPlayerHealth = 100f;

    [SerializeField] private float playerHealth;


    [SerializeField] private GameObject[] healthSegments;
    [SerializeField] private AudioSource hitsound;

    private ReworkedScreenFader fader;

    private void Start()
    {
        playerHealth = maxPlayerHealth;
        fader = FindAnyObjectByType<ReworkedScreenFader>();

        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        playerHealth = Mathf.Clamp(playerHealth - amount, 0, maxPlayerHealth);
        hitsound.Play();
        UpdateHealthUI();

        if (playerHealth <= 0) Death();
    }

    public void Heal(float amount)
    {
        playerHealth = Mathf.Clamp(playerHealth + amount, 0, maxPlayerHealth);

        UpdateHealthUI();
    }

    private void Death()
    {
        GameManager.Instance.CurrentState = GameManager.GameState.GameOver;

        fader.DoFadeIn(() => {
            SceneManager.LoadScene("DarkBox");
        }, Color.red);
    }

    private void UpdateHealthUI()
    {
        int segmentsToShow = Mathf.CeilToInt(playerHealth / (maxPlayerHealth / healthSegments.Length));

        for (int i = 0; i < healthSegments.Length; i++)
        {
            if (i < segmentsToShow)
                healthSegments[i].SetActive(true);
            else
                healthSegments[i].SetActive(false);
        }
    }
}