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
        if (!other.CompareTag("Player")) return;

        SpawnBoss();
    }

    public void SpawnBoss()
    {
        if (_bossHasSpawned) return;
        if (isNetworked) NetworkServer.Spawn(Instantiate(boss, spawnPoint.position, spawnPoint.rotation));
            else Instantiate(boss, spawnPoint.position, spawnPoint.rotation);
        
        _bossHasSpawned = true;
    }
}
