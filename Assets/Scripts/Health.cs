using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    private static int currentHealth;
    [SerializeField] ParticleSystem hit;
    
    void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        
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
        EnemyBase.ChangeState(EnemyBase.EnemyState.Dead);
    }
}
