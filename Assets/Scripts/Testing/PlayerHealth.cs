using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private int maxHealth = 100;

    private int health;

    public int currentHealth
    {
        get { return health; }
        set { health = Mathf.Clamp(value, 0, maxHealth); }
    }
}
