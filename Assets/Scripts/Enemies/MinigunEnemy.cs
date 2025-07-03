using System;
using UnityEngine;

public class MinigunEnemy : EnemyBase
{
    [SerializeField] private Transform[] shootpoints;
    [SerializeField] private int Range;
    [SerializeField] private LayerMask playerlayer;
    [SerializeField] private int damageamount;
    [SerializeField] private AudioSource Shoot;
    
    protected override void UpdateAttack()
    {
        UpdateChase();
        if (Time.time - lastAttackTime < AttackRate)
            return;

        lastAttackTime = Time.time;
        Shoot.Play();
        
        for (int i = 0; i < shootpoints.Length; i++)
        {
            Ray ray = new Ray(shootpoints[i].position, shootpoints[i].forward);
            if (Physics.Raycast(ray, out RaycastHit hit, Range, playerlayer))
            {
                PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damageamount);
                }
            }
            Debug.DrawRay(shootpoints[i].position, shootpoints[i].forward * Range, Color.red, 0.2f);
        }
    }

    protected override void UpdateDead()
    {
        Shoot.Stop();
        base.UpdateDead();
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < shootpoints.Length; i++)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(shootpoints[i].position,  shootpoints[i].forward * Range);
        }
    }
    
    public override void Die()
    {
        
        
    }
}
