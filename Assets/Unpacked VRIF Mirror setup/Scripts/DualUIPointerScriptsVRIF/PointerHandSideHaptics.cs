using UnityEngine;
using BNG;

public class PointerHandSideHaptics : MonoBehaviour
{
    [Header("Controller Haptic Settings")]
    public float VibrateFrequency = 1f;
    public float VibrateAmplitude = 1f;
    public float VibrateDuration = 2f;
    // run this from PointerEvents Component on the object
    VRUISystem vrUI;
    
    public void Start()
    {
        vrUI = VRUISystem.Instance;
    }
    
    public void PointerEvent()
    {
        ControllerHand handSide = vrUI.SelectedHand;
        DoHaptics(handSide);
    }

    private void DoHaptics(ControllerHand handSide)
    {
        InputBridge.Instance.VibrateController(VibrateFrequency, VibrateAmplitude, VibrateDuration, handSide);
    }
}
