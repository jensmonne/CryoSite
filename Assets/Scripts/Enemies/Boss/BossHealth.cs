using System;
using System.Collections;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int MaxHealth;
    public int currentHealth;
    [SerializeField] private ParticleSystem hit;
    [SerializeField] private BossBehavior boss;
    [SerializeField] private BossHealthBar healthBar;
    private OnBossTriggerEnter onBossTriggerEnter;
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private float flashDuration = 0.2f;
    private Material[][] materials;

    private void Start()
    {
        currentHealth = MaxHealth;
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

    public void CmdTakeDamage(int damage)
    {
        currentHealth -= damage;
        hit.Play();
        StartCoroutine(FlashRoutine());
        if (currentHealth <= 0) Death();
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
        onBossTriggerEnter.BossDied();
        Debug.Log("Killed Boss");
        boss.ChangeState(BossBehavior.BossState.Death);
    }
}
