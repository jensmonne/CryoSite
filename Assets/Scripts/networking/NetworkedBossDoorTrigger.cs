using UnityEngine;
using Mirror;

public class NetworkedBossDoorTrigger : NetworkBehaviour
{
    [SerializeField] private NetworkedBosDoor bosDoor;
    
    [SerializeField] private OnBossTriggerEnter obte; // Script that handles boss spawning
    [SerializeField] private GameObject arena; 

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;

        if (other.CompareTag("Key")) 
        {
            bosDoor.SetCanAutoOpen(true); 
            arena.SetActive(true);
            // Spawn the boss
            obte.SpawnBoss();
        }
    }
}