using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [SerializeField] private int MaxHealth;
    public int currentHealth;
    [SerializeField] private ParticleSystem hit;
    [SerializeField] private BossBehavior boss;
    private OnBossTriggerEnter onBossTriggerEnter;

    private void Start()
    {
        currentHealth = MaxHealth;
        onBossTriggerEnter = FindObjectOfType<OnBossTriggerEnter>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        hit.Play();
        if (currentHealth <= 0) Death();
    }

    private void Death()
    {
        onBossTriggerEnter.BossDied();
        Debug.Log("Killed Boss");
        boss.ChangeState(BossBehavior.BossState.Death);
    }
}
