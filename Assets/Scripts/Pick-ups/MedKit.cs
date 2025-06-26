using UnityEngine;

public class MedKit : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private float Healingamount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponent<PlayerHealth>();
            playerHealth.Heal(Healingamount);
            Destroy(gameObject);
        }
    }
}
