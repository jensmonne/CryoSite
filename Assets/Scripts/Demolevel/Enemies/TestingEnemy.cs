using UnityEngine;

public class TestingEnemy : EnemyBase
{
    [SerializeField] private float maxHealth = 100;
    
    [SerializeField] private float health;

    private void Start()
    {
        health = maxHealth;
    }

    public float CurrentHealth
    {
        get { return health; }
        set { health -= Mathf.Clamp(value, 0, health); }
    }

    public override void Die()
    {
        Destroy(gameObject);
    }

    // Uncomment if we want the damage to handle the collision instead of the bullet or whatever collided with it
    /*private void OnCollisionEnter(Collision collision)
    {
        CurrentHealth = collision.relativeVelocity.magnitude;
    }*/
}
