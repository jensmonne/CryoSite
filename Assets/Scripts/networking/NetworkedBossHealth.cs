using Mirror;
using UnityEngine;

public class NetworkedBossHealth : NetworkBehaviour
{
    [SerializeField] private int MaxHealth = 500;
    
    public int currentHealth;
    
    [SerializeField] private ParticleSystem hit;
    [SerializeField] private BossBehavior boss;

    private void Start()
    {
        currentHealth = MaxHealth;
    }

    [Server]
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        RpcPlayHitEffect();
        if (currentHealth <= 0) Death();
    }
    
    [ClientRpc]
    private void RpcPlayHitEffect()
    {
        if (hit != null) hit.Play();
    }

    [Server]
    private void Death()
    {
        Debug.Log("Killed Boss");
        //boss.ChangeState(NetworkedBossBehavior.BossState.Death);
    }
    
    [Command]
    public void CmdRequestDamage(int amount)
    {
        TakeDamage(amount);
    }
}
