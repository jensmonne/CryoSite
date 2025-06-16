using BNG;
using UnityEngine;

public class EndingStuff : MonoBehaviour
{
    private ScreenFader fader;
    
    private void Start()
    {
        fader = FindObjectOfType<ScreenFader>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!GameManager.IsBombActive) return;

        fader.DoFadeIn();
    }
}
