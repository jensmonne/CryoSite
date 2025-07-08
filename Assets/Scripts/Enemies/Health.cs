using System;
using System.Collections;
using Mirror;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SyncVar]
    public int currentHealth;

    public AudioSource hit;
    [SerializeField] private Renderer[] renderers;
    public int maxHealth = 100;
    private Coroutine flashCoroutine;
    private Material[][] materials;
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private GameObject DeathParticles;

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
    }


    [Command(requiresAuthority = false)]
    public void CmdDealDamage(int damage)
    {
        Debug.Log($"[SERVER] CmdDealDamage received. Damage: {damage}");

        if (currentHealth <= 0) return;

        currentHealth -= damage;
        RpcOnHit();

        if (currentHealth <= 0)
        {
            RpcDeath();
        }
    }

    [ClientRpc]
    void RpcOnHit()
    {
        if (hit != null) hit.Play();

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    [ClientRpc]
    void RpcDeath()
    {
        Debug.Log("Enemy died.");
        Instantiate(DeathParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
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
}