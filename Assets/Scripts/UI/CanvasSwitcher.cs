using UnityEngine;

public class CanvasSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject settings;

    public void SettingsButton()
    {
        menu.SetActive(false);
        settings.SetActive(true);
    }

    public void Back()
    {
        menu.SetActive(true);
        settings.SetActive(false);
    }
}
