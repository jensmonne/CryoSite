using System;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [SerializeField] private int MaxHealth;
    public int currentHealth;
    [SerializeField] private ParticleSystem hit;
    [SerializeField] private BossBehavior boss;

    private void Start()
    {
        currentHealth = MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        hit.Play();
        if (currentHealth <= 0) Death();
    }

    private void Death()
    {
        Debug.Log("Killed Boss");
        boss.ChangeState(BossBehavior.BossState.Death);
        
    }
}
