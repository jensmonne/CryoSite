using Mirror;
using UnityEngine;
using VRIF_Mirror_Package.Scripts.UI.Utils;

public class NetworkPlayerHealth : NetworkBehaviour
{
    [SerializeField] private float maxPlayerHealth = 100f;

    [SerializeField] private float playerHealth;
    
    [SerializeField] private GameObject[] healthSegments;
    [SerializeField] private AudioSource hitsound;

    private ReworkedScreenFader fader;

    private void Start()
    {
        playerHealth = maxPlayerHealth;
        fader = FindAnyObjectByType<ReworkedScreenFader>();

        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        playerHealth = Mathf.Clamp(playerHealth - amount, 0, maxPlayerHealth);
        hitsound.Play();
        UpdateHealthUI();

        if (playerHealth <= 0)
        {
            if (isServer)
                HandleDeathOnServer();
            else
                CmdRequestDeath();
        }
    }

    public void Heal(float amount)
    {
        playerHealth = Mathf.Clamp(playerHealth + amount, 0, maxPlayerHealth);

        UpdateHealthUI();
    }
    
    [Command]
    private void CmdRequestDeath()
    {
        HandleDeathOnServer();
    }
    
    [Server]
    private void HandleDeathOnServer()
    {
        GameManager.Instance.CurrentState = GameManager.GameState.GameOver;

        RpcDoClientFade();

        NetworkManager.singleton.ServerChangeScene("DarkBox");
    }
    
    [ClientRpc]
    private void RpcDoClientFade()
    {
        if (fader == null)
            fader = FindAnyObjectByType<ReworkedScreenFader>();

        fader?.DoFadeIn(null, Color.red);
    }

    private void UpdateHealthUI()
    {
        int segmentsToShow = Mathf.CeilToInt(playerHealth / (maxPlayerHealth / healthSegments.Length));

        for (int i = 0; i < healthSegments.Length; i++)
        {
            if (i < segmentsToShow)
                healthSegments[i].SetActive(true);
            else
                healthSegments[i].SetActive(false);
        }
    }
}