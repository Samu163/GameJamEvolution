using UnityEngine;

[System.Serializable]
public class SoundTrack
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
    public bool loop = true;
    public float fadeInDuration = 1f;
    public float fadeOutDuration = 1f;
    
    // For layered/adaptive music
    public AudioClip[] layers;
    [Range(0f, 1f)]
    public float[] layerVolumes;
} 