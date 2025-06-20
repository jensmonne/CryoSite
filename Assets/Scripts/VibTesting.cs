using System.Collections;
using BNG;
using UnityEngine;

public class VibTesting : MonoBehaviour
{
    public float VibrateFrequency = 1f;
    public float VibrateAmplitude = 1f;
    public float VibrateDuration = 0.1f;
    public float RepeatDelay = 0.05f;

    private void Start()
    {
        StartCoroutine(LoopVibration());
    }

    private IEnumerator LoopVibration()
    {
        while (true)
        {
            InputBridge.Instance.VibrateController(VibrateFrequency, VibrateAmplitude, VibrateDuration,
                ControllerHand.Left);
            InputBridge.Instance.VibrateController(VibrateFrequency, VibrateAmplitude, VibrateDuration,
                ControllerHand.Right);
            yield return new WaitForSeconds(VibrateDuration + RepeatDelay);
        }
    }
}
