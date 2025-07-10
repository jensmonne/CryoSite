using UnityEngine;

public class SawEnemy : EnemyBase
{
    [SerializeField] private PlayerHealth health;
    [SerializeField] private NetworkPlayerHealth playerHealth;
    [SerializeField] private bool dealDamage = false;
    [SerializeField] private int damage = 10;
    [SerializeField] private Animator animator;
    [SerializeField] private float attackRate = 1f;
    private float attackTimer = 0f;

    protected override void Update()
    {
        base.Update();

        if (health == null)
        {
            health = FindObjectOfType<PlayerHealth>();
            playerHealth = FindObjectOfType<NetworkPlayerHealth>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dealDamage = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        dealDamage = false;
        attackTimer = 0f;
    }

    protected override void UpdateAttack()
    {
        UpdateChase();

        if (!dealDamage || health == null) return;

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackRate)
        {
            health.TakeDamage(damage);
            playerHealth.TakeDamage(damage);
            attackTimer = 0f;
        }
    }

    public override void Die()
    {
        // Implement death logic here
    }
}