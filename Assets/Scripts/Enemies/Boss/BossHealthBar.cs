using System;
using UnityEngine;

public class BossHealthBar : MonoBehaviour
{
    private BossHealth bossHealth;
    private void Start()
    {
        bossHealth = GetComponent<BossHealth>();
    }

    [SerializeField] private GameObject[] healthSegments;
    
    public void UpdateHealthUI()
    {
        int segmentsToShow = Mathf.CeilToInt(bossHealth.currentHealth / (bossHealth.MaxHealth / healthSegments.Length));

        for (int i = 0; i < healthSegments.Length; i++)
        {
            if (i < segmentsToShow)
                healthSegments[i].SetActive(true);
            else
                healthSegments[i].SetActive(false);
        }
    }
}
