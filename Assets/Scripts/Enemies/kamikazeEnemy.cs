using System.Collections;
using UnityEngine;

public class kamikazeEnemy : EnemyBase
{
    [SerializeField] private float explosionRange = 5f;
    [SerializeField] private int explosionDamage = 50;
    private bool Exploded = false;

    protected override void Start()
    {
        base.Start();
        Exploded = false;
    }

    protected override void UpdateAttack()
    {
        if (!Exploded)
        {
            Exploded = true;
            StartCoroutine(ExplodeAfterDelay());
        }
    }
    
    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(0.1f); 
        ChangeState(EnemyState.Dead);
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
            
            var networkhealth = collider.GetComponent<NetworkedHealthEnemy>();
            if (networkhealth != null)
            {
                networkhealth.CmdDealDamage(explosionDamage);
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
        if (!Exploded)
        {
            Exploded = true;
            ChangeState(EnemyState.Dead);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
