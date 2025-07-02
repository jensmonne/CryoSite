using System;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class SawEnemy : EnemyBase
{

    [SerializeField] private PlayerHealth health;
    [SerializeField] private bool Dealdamage;
    [SerializeField] private int Damage;
    [SerializeField] private Animator animator;
    [SerializeField] private float attackRate = 1f;
    private float attacktimer;

    protected override void Update()
    {
        base.Update();
        if (health == null)
        {
            health = FindObjectOfType<PlayerHealth>();

        }
    }
    
    protected void OnTriggerEnter(Collider other)
    {
        if (CompareTag("Player"))
        {
            Dealdamage = true;
        }

    }

    protected void OnTriggerExit(Collider other)
    {
        if (CompareTag("Player"))
        {
            Dealdamage = false;
        }
    }

    protected override void UpdateAttack()
    {
        if (Dealdamage == false) return;

        attacktimer += Time.deltaTime;

        if ( attacktimer>= attackRate)
        {
            health.TakeDamage(Damage);
            attacktimer = 0f;
        }
    }

    public override void Die()
    {
        throw new System.NotImplementedException();
    }
}
