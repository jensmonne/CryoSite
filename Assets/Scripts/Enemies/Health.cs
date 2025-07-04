using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    private int currentHealth;
    [SerializeField] private ParticleSystem hit;
    [SerializeField] private GameObject explosion;
    [SerializeField] private EnemyBase enemyScript;
    [SerializeField] private AudioSource explodeSound;
    [SerializeField] private Renderer objectRenderer;
    [SerializeField] private float flashDuration = 0.2f;

    private Material mat;

    private void Start()
    {
        currentHealth = maxHealth;
        mat = objectRenderer.material;
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("TakeDamage");
        currentHealth -= damage;
        
        if (hit != null)
            hit.Play();

        mat.SetFloat("FlashAmount", 1f);
        StartCoroutine(FadeFlash());

        if (currentHealth <= 0)
            Death();
    }

    private IEnumerator FadeFlash()
    {
        float elapsed = 0f;

        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            float t = 1f - (elapsed / flashDuration);
            mat.SetFloat("FlashAmount", t);
            yield return null;
        }

        mat.SetFloat("FlashAmount", 0f);
    }

    private void Death()
    {
        Debug.Log("You killed him why did you do that");

        if (explosion != null)
            Instantiate(explosion, transform.position, Quaternion.identity);

        if (explodeSound != null)
            explodeSound.Play();

        if (enemyScript != null)
            enemyScript.ChangeState(EnemyBase.EnemyState.Dead);
    }
}