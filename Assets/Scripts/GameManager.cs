using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static bool IsBombActive { get; set; }
    
    [SerializeField] private float bombDuration = 10f;
    
    public static float bombTimer { get; private set; }

    public float RemainingTime
    {
        get { return Mathf.Max(0f, bombDuration - bombTimer); }
        private set {}
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (IsBombActive)
        {
            bombTimer += Time.deltaTime;

            if (bombTimer >= bombDuration)
            {
                
            }
        }
    }
}
