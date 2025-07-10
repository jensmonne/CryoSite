using UnityEngine;

public class CanvasSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject connectUI;
    [SerializeField] private GameObject credits;

    public void SettingsButton()
    {
        menu.SetActive(false);
        settings.SetActive(true);
        connectUI.SetActive(false);
        credits.SetActive(false);
    }

    public void Back()
    {
        menu.SetActive(true);
        settings.SetActive(false);
        connectUI.SetActive(false);
        credits.SetActive(false);
    }
    
    public void StartConnect()
    {
        menu.SetActive(false);
        settings.SetActive(false);
        connectUI.SetActive(true);
        credits.SetActive(false);
    }
    
    public void CreditsButton()
    {
        menu.SetActive(false);
        settings.SetActive(false);
        connectUI.SetActive(false);
        credits.SetActive(true);
    }
}
