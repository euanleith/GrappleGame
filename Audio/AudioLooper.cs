using UnityEngine;

public class AudioLooper : MonoBehaviour {
    public float overlapTime = 4f;
    public float volume = 0.5f;
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
        Debug.Log("0: " + audioSources[0].time + " " + (audioSources[0].clip.length - overlapTime) + " " + audioSources[0].volume);
        Debug.Log("1: " + audioSources[1].time + " " + (audioSources[1].clip.length - overlapTime) + " " + audioSources[0].volume);

        if (audioSources[0].time >= audioSources[0].clip.length - overlapTime) {
            if (!audioSources[1].isPlaying) {
                Debug.Log("playing 2");
                audioSources[1].Play();
                elapsedTime = 0;
            }
            elapsedTime += Time.deltaTime;
                //audioSources[0].volume = volume * (audioSources[0].clip.length - audioSources[0].time);
                audioSources[0].volume = Mathf.Lerp(volume, 0, elapsedTime / overlapTime);
            audioSources[1].volume = Mathf.Lerp(0, volume, elapsedTime / overlapTime);
        }
        if (audioSources[1].time >= audioSources[1].clip.length - overlapTime) {
            //Debug.Log(audioSources[1].time);
            if (!audioSources[0].isPlaying) {
                Debug.Log("playing 1");
                audioSources[0].Play();
                elapsedTime = 0;
            }
            elapsedTime += Time.deltaTime;
            audioSources[0].volume = Mathf.Lerp(0, volume, elapsedTime / overlapTime);
            audioSources[1].volume = Mathf.Lerp(volume, 0, elapsedTime / overlapTime);
        }
    }
}