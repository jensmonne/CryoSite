using Mirror;
using UnityEngine;

public class OnBossTriggerEnter : MonoBehaviour
{
    [SerializeField] private GameObject boss;
    [SerializeField] private Transform spawnPoint;
    [Tooltip("If it's the networked scene check this, if not uncheck this otherwise it will ehh err")]
    [SerializeField] private bool isNetworked = false;
    private bool Bosshasspawned = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") && !Bosshasspawned) return;
        
        if (isNetworked) NetworkedBossSpawn();
        else BossSpawn();
        Bosshasspawned = true;
    }

    private void BossSpawn()
    {
        Instantiate(boss, spawnPoint.position, spawnPoint.rotation);
    }
    
    private void NetworkedBossSpawn()
    {
        NetworkServer.Spawn(Instantiate(boss, spawnPoint.position, spawnPoint.rotation));
    }
}
