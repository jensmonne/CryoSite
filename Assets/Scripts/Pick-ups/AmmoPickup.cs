using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    private GameManager GM;
    [SerializeField] private AudioSource audioSource;
    
    private void Update()
    {
        if (GM == null)
        {
            GM = FindFirstObjectByType<GameManager>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        GM.AddMag();
        audioSource.Play();
        Destroy(gameObject);
    }
}
