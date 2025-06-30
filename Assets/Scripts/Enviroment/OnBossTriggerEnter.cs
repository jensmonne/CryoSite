using Mirror;
using UnityEngine;

public class OnBossTriggerEnter : MonoBehaviour
{
    [SerializeField] private GameObject boss;
    [SerializeField] private Transform spawnPoint;
    [Tooltip("If it's the networked scene check this, if not uncheck this otherwise it will ehh err")]
    [SerializeField] private bool isNetworked = false;
    private bool _bossHasSpawned = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") && !_bossHasSpawned) return;
        
        if (isNetworked) NetworkedBossSpawn();
        else BossSpawn();
    }

    private void BossSpawn()
    {
        Instantiate(boss, spawnPoint.position, spawnPoint.rotation);
        _bossHasSpawned = true;
    }
    
    private void NetworkedBossSpawn()
    {
        NetworkServer.Spawn(Instantiate(boss, spawnPoint.position, spawnPoint.rotation));
        _bossHasSpawned = true;
    }
}
