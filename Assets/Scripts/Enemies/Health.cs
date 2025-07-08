using UnityEngine;
using System.Collections;
using Mirror;

public class Health : NetworkBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SyncVar] private int currentHealth;

    [Header("FX References")]
    [SerializeField] private ParticleSystem hit;
    [SerializeField] private GameObject explosion;
    [SerializeField] private AudioSource explodeSound;

    [Header("Flash Material Settings")]
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private float flashDuration = 0.2f;

    [Header("Optional: Enemy Script")]
    [SerializeField] private EnemyBase enemyScript;

    private Material[][] materials;

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
    }

    public override void OnStartClient()
    {
        SetupMaterials();
    }

#if UNITY_EDITOR
    void Update()
    {
        // Testing damage locally in editor on Server
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (isServer)
            {
                CmdTakeDamage(10);
            }
            else
            {
                SimulateDamageOffline(10);
            }
        }
    }
#endif

    private void SetupMaterials()
    {
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

    private void SetFlashAmount(float amount)
    {
        if (materials == null) return;

        for (int i = 0; i < materials.Length; i++)
        {
            for (int j = 0; j < materials[i].Length; j++)
            {
                materials[i][j].SetFloat("_FlashAmount", amount);
            }
        }
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

    [Command]
    public void CmdTakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        RpcPlayHitEffects();

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    [ClientRpc]
    private void RpcPlayHitEffects()
    {
        if (hit != null)
            hit.Play();

        if (materials == null)
            SetupMaterials();

        StartCoroutine(FlashRoutine());
    }

    [Server]
    private void HandleDeath()
    {
        RpcHandleDeathEffects();

        if (enemyScript != null)
        {
            enemyScript.ChangeState(EnemyBase.EnemyState.Dead);
        }

        // Optionally destroy on server:
        // NetworkServer.Destroy(gameObject);
    }

    [ClientRpc]
    private void RpcHandleDeathEffects()
    {
        if (explodeSound != null)
            explodeSound.Play();

        if (explosion != null)
            Instantiate(explosion, transform.position, Quaternion.identity);
    }

    private void SimulateDamageOffline(int damage)
    {
        currentHealth -= damage;
        if (hit != null)
            hit.Play();

        if (materials == null)
            SetupMaterials();

        StartCoroutine(FlashRoutine());

        if (currentHealth <= 0)
        {
            if (explodeSound != null)
                explodeSound.Play();

            if (explosion != null)
                Instantiate(explosion, transform.position, Quaternion.identity);

            if (enemyScript != null)
                enemyScript.ChangeState(EnemyBase.EnemyState.Dead);
        }
    }
}
