using Mirror;
using UnityEngine;

public class NetworkedTriggerSpawn : NetworkBehaviour
{
    [SerializeField] private SpawnZone[] spawnZones;
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;

        if (isServer)
        {
            SpawnAll();
        }
        else if (isClient)
        {
            CmdRequestSpawn(); // Client asks server to do the spawning
        }
    }
    
    [Command(requiresAuthority = false)]
    private void CmdRequestSpawn()
    {
        if (hasTriggered) return; // Double-check on server
        hasTriggered = true;
        SpawnAll();
    }

    [Server]
    private void SpawnAll()
    {
        foreach (var zone in spawnZones)
        {
            if (zone != null)
            {
                zone.SpawnEnemies(); // This must use NetworkServer.Spawn()
            }
        }
    }
}