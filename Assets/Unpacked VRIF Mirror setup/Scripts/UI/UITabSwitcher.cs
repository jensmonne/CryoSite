using UnityEngine;

public class UITabSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject connectPanel, lobbyPanel, settingsPanel;

    public void ShowConnect() 
    {
        connectPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void ShowLobby() 
    {
        Debug.LogError("this got called");
        connectPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void ShowSettings()
    {
        connectPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
}