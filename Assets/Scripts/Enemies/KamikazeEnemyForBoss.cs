using System.Collections;
using UnityEngine;
using Mirror;

public class kamikazeEnemyBoss : EnemyBase
{
    [SerializeField] private float explosionRange = 1f;
    [SerializeField] private int explosionDamage = 25;
    private bool Exploded = false;

    protected override void Start()
    {
        base.Start();
        Exploded = false;
    }

    protected override void UpdateAttack()
    {
        if (!isServer || Exploded) return; // Server handles attack

        Exploded = true;
        StartCoroutine(ExplodeAfterDelay());
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        ChangeState(EnemyState.Dead); // This will trigger UpdateDead
    }

    protected override void UpdateDead()
    {
        if (!isServer) return; // Only run explosion damage on the server

        base.UpdateDead();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRange);

        foreach (Collider collider in hitColliders)
        {
            // Try to damage normal Health
            if (collider.TryGetComponent(out Health health))
            {
                health.CmdTakeDamage(explosionDamage); // Must be network-safe
                continue;
            }

            // Try to damage player health
            if (collider.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(explosionDamage); // Same as above
            }
        }
    }

    public override void Die()
    {
        if (!isServer || Exploded) return;

        Exploded = true;
        ChangeState(EnemyState.Dead); // Trigger explosion
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}