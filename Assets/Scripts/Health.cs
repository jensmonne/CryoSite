using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    private int currentHealth;
    [SerializeField] ParticleSystem hit;
    [SerializeField] private kamikazeEnemy kamikaze;
    
    void Start()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        Debug.Log("TakeDamage");
        currentHealth -= damage;
        hit.Play();
        if (currentHealth <= 0)
        {
            Death();
        }

    }

    private void Death()
    {
        Debug.Log("You killed him why did you do that");
        kamikaze.ChangeState(EnemyBase.EnemyState.Dead);
    }
}
