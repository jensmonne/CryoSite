using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnZone : MonoBehaviour
{ 
    public GameObject[] enemies;

    [SerializeField] private bool isNetworked = false;
    [SerializeField] private int enemycount = 10;
    public Vector3 spawnArea;

    private void Start()
    {
        SpawnEnemies();
    }

    public void SpawnEnemies()
    {
        for (int i = 0; i < enemycount; i++)
        {
            Vector3 spawnpos = transform.position + new Vector3(
                Random.Range(-spawnArea.x, spawnArea.x),
                Random.Range(-spawnArea.y, spawnArea.y),
                Random.Range(-spawnArea.z, spawnArea.z)
            );
            GameObject selectedEnemy = enemies[Random.Range(0, enemies.Length)];
            if (isNetworked)
            {
                NetworkServer.Spawn(Instantiate(selectedEnemy, spawnpos, Quaternion.identity));
            }
            else Instantiate(selectedEnemy, spawnpos, Quaternion.identity);
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, spawnArea);
    }
}
