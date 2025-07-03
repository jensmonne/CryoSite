using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    private int currentHealth;
    [SerializeField] ParticleSystem hit;
    [SerializeField] GameObject explosion;
    [SerializeField] private EnemyBase Enemyscirpt;
    [SerializeField] private AudioSource ExplodeSound;
    
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
        Instantiate(explosion, transform.position, Quaternion.identity);
        ExplodeSound.Play();
        Enemyscirpt.ChangeState(EnemyBase.EnemyState.Dead);
    }
}
