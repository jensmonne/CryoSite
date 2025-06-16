using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxPlayerHealth = 100;

    [SerializeField] private float playerHealth;

    private void Start()
    {
        playerHealth = maxPlayerHealth;
    }

    public void TakeDamage(float amount)
    {
        playerHealth = Mathf.Clamp(playerHealth - amount, 0, maxPlayerHealth);
    }

    public void Heal(float amount)
    {
        playerHealth = Mathf.Clamp(playerHealth + amount, 0, maxPlayerHealth);
    }
}
