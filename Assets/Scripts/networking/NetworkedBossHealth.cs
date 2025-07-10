using System.Collections;
using Mirror;
using UnityEngine;

public class NetworkedBossHealth : NetworkBehaviour
{
    [SyncVar]
    public int currentHealth;
 
    public int MaxHealth;
    [SerializeField] private ParticleSystem hit;
    [SerializeField] private NetworkedBossBehavior boss;
    [SerializeField] private BossHealthBar healthBar;
    private OnBossTriggerEnter onBossTriggerEnter;
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private float flashDuration = 0.2f;
    private Material[][] materials;
    private Coroutine flashCoroutine;
    [SerializeField] private GameObject DeathParticles;

    public override void OnStartServer()
    {
        currentHealth = MaxHealth;
    }
    
    private void Start()
    {
        onBossTriggerEnter = FindObjectOfType<OnBossTriggerEnter>();
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

    private void Update()
    {
        healthBar.UpdateHealthUI();
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
        onBossTriggerEnter.BossDied();
        Instantiate(DeathParticles, transform.position, Quaternion.identity);
        boss.ChangeState(NetworkedBossBehavior.BossState.Death);
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