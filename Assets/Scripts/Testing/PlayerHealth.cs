using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxPlayerHealth = 100;

    [SerializeField] private float playerHealth;

    private void Start()
    {
        playerHealth = maxPlayerHealth;
    }

    public float CurrentPlayerHealth
    {
        get { return playerHealth; }
        set { playerHealth -= Mathf.Clamp(value, 0, maxPlayerHealth); }
    }
}
