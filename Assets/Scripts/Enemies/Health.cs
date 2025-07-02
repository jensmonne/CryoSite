using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    private int currentHealth;
    [SerializeField] ParticleSystem hit;
    [SerializeField] ParticleSystem death;
    [SerializeField] private EnemyBase Enemyscirpt;
    
    private void Start()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        Debug.Log("TakeDamage");
        currentHealth -= damage;
        hit.Play();
        if (currentHealth <= 0) Death();

    }
    
    private void Death()
    {
        Debug.Log("You killed him why did you do that");
        death.Play();
        Enemyscirpt.ChangeState(EnemyBase.EnemyState.Dead);
    }
}
