using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    private GameManager GM;
    
    void Update()
    {
        if (GM == null)
        {
            GM = FindFirstObjectByType<GameManager>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GM.AddMag();
            Destroy(gameObject);
        }

    }
}
