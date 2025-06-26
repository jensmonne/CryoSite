using BNG;
using UnityEngine;

namespace VRIF_Mirror_Package.Scripts.DualUIPointerScriptsVRIF
{
    public class PointerHandSideHaptics : MonoBehaviour
    {
        [Header("Controller Haptic Settings")]
        public float vibrateFrequency = 0.3f;
        public float vibrateAmplitude = 0.1f;
        public float vibrateDuration = 0.1f;
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
            InputBridge.Instance.VibrateController(vibrateFrequency, vibrateAmplitude, vibrateDuration, handSide);
        }
    }
}
