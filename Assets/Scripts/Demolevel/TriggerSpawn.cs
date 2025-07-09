using UnityEngine;

public class TriggerSpawn : MonoBehaviour
{
    [SerializeField] private SpawnZone[] spawnZones;
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;

        foreach (var zone in spawnZones)
        {
            if (zone != null)
            {
                zone.SpawnEnemies();
            }
        }
    }
}