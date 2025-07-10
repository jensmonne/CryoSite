using UnityEngine;

public class NetworkMedKit : MonoBehaviour
{
    [SerializeField] private NetworkPlayerHealth playerHealth;
    [SerializeField] private float healingAmount;
    [SerializeField] private AudioSource healingSound;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        playerHealth = other.GetComponent<NetworkPlayerHealth>();
        playerHealth.Heal(healingAmount);
        healingSound.Play();
        Destroy(gameObject);
    }
}
