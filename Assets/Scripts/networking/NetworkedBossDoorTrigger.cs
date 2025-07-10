using UnityEngine;
using Mirror;

public class NetworkedBossDoorTrigger : NetworkBehaviour
{
    [SerializeField] private NetworkedBosDoor bosDoor;

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;

        if (other.CompareTag("Key")) 
        {
            Debug.Log("[KeyCardScan]" + other.name);
            bosDoor.SetCanAutoOpen(true); 
        }
    }
}