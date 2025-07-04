using System;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [SerializeField] private int MaxHealth;
    public int currentHealth;
    [SerializeField] private ParticleSystem hit;
    [SerializeField] private BossBehavior boss;
    private OnBossTriggerEnter onBossTriggerEnter;
    [SerializeField] private GameObject[] healthSegments;

    private void Start()
    {
        currentHealth = MaxHealth;
        onBossTriggerEnter = FindObjectOfType<OnBossTriggerEnter>();
        
    }

    private void Update()
    {
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        hit.Play();
        if (currentHealth <= 0) Death();
    }
    private void UpdateHealthUI()
    {
        int segmentsToShow = Mathf.CeilToInt(currentHealth / (MaxHealth / healthSegments.Length));

        for (int i = 0; i < healthSegments.Length; i++)
        {
            if (i < segmentsToShow)
                healthSegments[i].SetActive(true);
            else
                healthSegments[i].SetActive(false);
        }
    }

    private void Death()
    {
        onBossTriggerEnter.BossDied();
        Debug.Log("Killed Boss");
        boss.ChangeState(BossBehavior.BossState.Death);
    }
}
