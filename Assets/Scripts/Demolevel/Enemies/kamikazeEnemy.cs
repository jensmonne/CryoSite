using UnityEngine;
using UnityEngine.AI;

public class kamikazeEnemy : EnemyBase
{
    [SerializeField] private float explosionRange = 5f;
    [SerializeField] private int explosionDamage = 50;
    private bool Exploded = false;
    

    protected override void UpdatePatrol()
    {
        base.UpdatePatrol();
        agent.speed = 0.5f;
    }

    protected override void UpdateChase()
    {
        base.UpdateChase();
        agent.speed = 2.5f;
    }

    protected override void UpdateAttack()
    {
        if (!Exploded)
        {
            ChangeState(EnemyState.Dead);
            Exploded = true;
        }
    }

    protected override void UpdateDead()
    {
        base.UpdateDead();
        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRange);

        foreach (Collider collider in hitColliders)
        {

            var health = collider.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(explosionDamage);
                continue;
            }
            
            var playerHealth = collider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(explosionDamage);
            }
        }
        
    }

    public override void Die()
    {
        
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
