using UnityEngine;
using Random = UnityEngine.Random;

public class VoiceOverPlay : MonoBehaviour
{
    [SerializeField] private AudioSource[] audioSource;
    
    private void Start()
    {
        if (audioSource == null || audioSource.Length == 0) return;
        
        int randomIndex = Random.Range(0, audioSource.Length);
        AudioSource selectedAudio = audioSource[randomIndex];
        selectedAudio.Play();
    }
}
