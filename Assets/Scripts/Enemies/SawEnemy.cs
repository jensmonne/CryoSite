using System;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class SawEnemy : EnemyBase
{
    private PlayerHealth health;
    private bool Dealdamage;
    [SerializeField] private int Damage;
    
    protected void OnTriggerEnter(Collider other)
    {
        Dealdamage = true;
        health = other.GetComponent<PlayerHealth>();
    }

    protected void OnTriggerExit(Collider other)
    {
        Dealdamage = false;
        health = null;
    }

    protected override void UpdateAttack()
    {
      if (!Dealdamage) return;
      health.TakeDamage(Damage);
    }



    public override void Die()
    {
        throw new System.NotImplementedException();
    }
}
