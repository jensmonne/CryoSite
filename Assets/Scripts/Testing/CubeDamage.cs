using UnityEngine;

public class CubeDamage : MonoBehaviour
{
    [SerializeField] private int damage;
    
    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        PlayerHealth hpScript = other.gameObject.GetComponent<PlayerHealth>();
        if (hpScript == null) return;
        
        hpScript.CurrentPlayerHealth = damage;
    }
}
