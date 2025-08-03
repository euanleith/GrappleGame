using System.Diagnostics;
using UnityEngine;

public class AudioLooper : MonoBehaviour {
    public float overlapTime = 4f;
    public float volume = 0.5f;
    public float minVolume = 0f; // todo replace with 'smoothing'? or volumeRange (0-1)?
    private AudioSource[] audioSources;
    private float elapsedTime = 0;

    private void Start() 
    {
        audioSources = GetComponents<AudioSource>();
        audioSources[0].volume = volume;
        audioSources[1].volume = volume;
        audioSources[0].Play();
    }

    private void Update() 
    {
        Overlap(audioSources[0], audioSources[1]);
        Overlap(audioSources[1], audioSources[0]);
    }

    private void Overlap(AudioSource audioSource1, AudioSource audioSource2) {
        if (audioSource1.time >= audioSource1.clip.length - overlapTime) {
            if (!audioSource2.isPlaying) {
                audioSource2.Play();
                elapsedTime = 0;
            }
            elapsedTime += Time.deltaTime;
            audioSource1.volume = Mathf.Lerp(volume, minVolume, elapsedTime / overlapTime);
            audioSource2.volume = Mathf.Lerp(minVolume, volume, elapsedTime / overlapTime);
        }
    }
}