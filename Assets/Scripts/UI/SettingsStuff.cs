using UnityEngine;

public class SettingsStuff : MonoBehaviour
{
    [SerializeField] private VRSettingsManager vrSettingsManager;
    [SerializeField] private UnityEngine.UI.Slider slider;
    [SerializeField] private GameObject menu;
    
    public void TurnSetting()
    {
        if (slider.value == 0)
        {
            vrSettingsManager.TurnSetting = TurnType.Snap;
        }
        else
        {
            vrSettingsManager.TurnSetting = TurnType.Smooth;
        }
    }

    public void Back()
    {
        menu.SetActive(true);
        gameObject.SetActive(false);
    }
}
