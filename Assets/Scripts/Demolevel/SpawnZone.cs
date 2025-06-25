using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnZone : MonoBehaviour
{ 
    public GameObject[] enemies;

    [SerializeField]private int enemycount = 10;
    public Vector3 SpawnArea;

    void Start()
    {
        SpawnEnemies();
    }

    public void SpawnEnemies()
    {
        for (int i = 0; i < enemycount; i++)
        {
            Vector3 spawnpos = transform.position + new Vector3(
                Random.Range(-SpawnArea.x, SpawnArea.x),
                Random.Range(-SpawnArea.y, SpawnArea.y),
                Random.Range(-SpawnArea.z, SpawnArea.z)
            );
            GameObject selectedEnemy = enemies[Random.Range(0, enemies.Length)];
            GameObject enemy = Instantiate(selectedEnemy,spawnpos,Quaternion.identity);
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, SpawnArea);
    }
}
