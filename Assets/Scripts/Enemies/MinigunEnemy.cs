using BNG;
using UnityEngine;

public class MinigunEnemy : EnemyBase
{
    [SerializeField] private Transform[] shootpoints;
    [SerializeField] private ParticleSystem[] particle;
    [SerializeField] private int Range;
    [SerializeField] private LayerMask playerlayer;
    [SerializeField] private int damageamount;
    [SerializeField] private AudioSource Shoot;
    [SerializeField] private float distanceToPlayer;
    
    protected override void UpdateAttack()
    {
        UpdateChase();
        if (Time.time - lastAttackTime < AttackRate)
            return;

        lastAttackTime = Time.time;
        if (!Shoot.isPlaying)
        {
            Shoot.Play();
        }

        for (int j = 0; j < particle.Length; j++)
        {
            particle[j].Play();
        }

        for (int i = 0; i < shootpoints.Length; i++)
        {
            Ray ray = new Ray(shootpoints[i].position, shootpoints[i].forward);
            if (Physics.Raycast(ray, out RaycastHit hit, Range, playerlayer))
            {
                NetworkPlayerHealth playerHp = hit.collider.GetComponent<NetworkPlayerHealth>();
                PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
                if (playerHealth != null) playerHealth.TakeDamage(damageamount);
                if (playerHp != null) playerHp.TakeDamage(damageamount);
            }
            Debug.DrawRay(shootpoints[i].position, shootpoints[i].forward * Range, Color.red, 0.2f);
        }
    }

    protected override void UpdateDead()
    {
        Shoot.Stop();
        base.UpdateDead();
    }
    
    
    public override void Die()
    {
        
    }
}
