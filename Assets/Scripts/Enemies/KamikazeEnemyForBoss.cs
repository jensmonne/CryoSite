using UnityEngine;
using UnityEngine.AI;

public class KamikazeEnemyForBoss : EnemyBase
{
    [SerializeField] private float explosionRange = 5f;
    [SerializeField] private int explosionDamage = 50;
    private bool exploded = false;
    [SerializeField] private AudioSource explosion;

    protected override void Start()
    {
        base.Start();
        exploded = false;
    }

    protected override void SetState()
    {
        if (currentState == EnemyState.Dead) return;
        if (Player == null) return;

        float distance = Vector3.Distance(transform.position, Player.position);

        if (distance < AttackDistance)
        {
            ChangeState(EnemyState.Attack);
        }
        else if (distance < FollowDistance)
        {
            ChangeState(EnemyState.Chase);
        }
        else
        {
            ChangeState(EnemyState.Idle);
        }
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