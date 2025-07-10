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

        if (!isServer)
        {
            CmdRequestSpawn(); // Client asks server to do the spawning
        }
        else
        {
            SpawnAll();
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
        Debug.LogError("spawned");
        foreach (var zone in spawnZones)
        {
            if (zone != null)
            {
                zone.SpawnEnemies(); // This must use NetworkServer.Spawn()
            }
        }
    }
}