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
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private float flashDuration = 0.2f;

    private Material[][] materials;

    private void Start()
    {
        currentHealth = maxHealth;
        materials = new Material[renderers.Length][];
        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] mats = renderers[i].materials;
            for (int j = 0; j < mats.Length; j++)
            {
                mats[j] = Instantiate(mats[j]);
            }
            renderers[i].materials = mats;
            materials[i] = mats;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (hit != null)
            hit.Play();
        StartCoroutine(FlashRoutine());
        if (currentHealth <= 0)
            Death();
    }

    private IEnumerator FlashRoutine()
    {
        SetFlashAmount(1f);
        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            float t = 1f - (elapsed / flashDuration);
            SetFlashAmount(t);
            yield return null;
        }
        SetFlashAmount(0f);
    }

    private void SetFlashAmount(float amount)
    {
        for (int i = 0; i < materials.Length; i++)
        {
            for (int j = 0; j < materials[i].Length; j++)
            {
                materials[i][j].SetFloat("_FlashAmount", amount);
            }
        }
    }

    private void Death()
    {
        if (explosion != null)
            Instantiate(explosion, transform.position, Quaternion.identity);
        if (explodeSound != null)
            explodeSound.Play();
        if (enemyScript != null)
            enemyScript.ChangeState(EnemyBase.EnemyState.Dead);
    }
}