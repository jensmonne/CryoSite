using System;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bossPrefab;
    private bool BossSpawned = false;

    private void Start()
    {
        BossSpawned = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !BossSpawned)
        {
            Instantiate(bossPrefab, transform.position, Quaternion.identity);
            BossSpawned = true;
        }
    }
}
