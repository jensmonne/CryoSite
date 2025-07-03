using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class VoiceOverPlay : MonoBehaviour
{
    [SerializeField] private AudioSource[] audioSource;


    private void Start()
    {
        int randomIndex = Random.Range(0, audioSource.Length);
        
        AudioSource selectedAudio = audioSource[randomIndex];
        
        selectedAudio.Play();
    }
}
