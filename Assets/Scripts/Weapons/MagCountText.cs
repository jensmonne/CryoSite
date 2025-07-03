using TMPro;
using UnityEngine;

public class MagCountText : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField]private TextMeshProUGUI MagText;
    
    void Update()
    {
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }

        MagText.text = gameManager? gameManager.Magcount.ToString() : "";
    }
}
