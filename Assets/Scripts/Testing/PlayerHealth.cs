using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private int maxHealth = 100;

    [SerializeField] private int health;

    private void Start()
    {
        health = maxHealth;
    }

    public int CurrentHealth
    {
        get { return health; }
        set { health -= Mathf.Clamp(value, 0, maxHealth); }
    }
}
