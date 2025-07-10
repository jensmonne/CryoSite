using UnityEngine;


public class KamikazeEnemyForBoss : EnemyBase
{
    [SerializeField] private float explosionRange = 5f;
    [SerializeField] private int explosionDamage = 50;
    private bool exploded;
    [SerializeField] private AudioSource explosion;

    protected override void Start()
    {
        base.Start();
        exploded = false;
    }

    protected override void UpdatePatrol()
    {
    }

    protected override void UpdateAttack()
    {
        if (!exploded)
        {
            ChangeState(EnemyState.Dead);
            exploded = true;
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    protected override void UpdateDead()
    {
        base.UpdateDead();
        explosion.Play();
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
            
            NetworkPlayerHealth playerHp = collider.GetComponent<NetworkPlayerHealth>();
            PlayerHealth playerHealth = collider.GetComponent<PlayerHealth>();
            if (playerHealth != null) playerHealth.TakeDamage(explosionDamage);
            if (playerHp != null) playerHp.TakeDamage(explosionDamage);
        }
    }

    public override void Die()
    {
    }
}