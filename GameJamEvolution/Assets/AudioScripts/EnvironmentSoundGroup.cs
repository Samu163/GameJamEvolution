using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Environment Sound Group", menuName = "Audio/Environment Sound Group")]
public class EnvironmentSoundGroup : ScriptableObject
{
    public string groupName;
    public bool playSequentially = false;
    public bool isLoopingGroup = true; // Whether this group contains looping ambient sounds or one-shot effects
    public List<EnvironmentSound> sounds = new List<EnvironmentSound>();
    
    [HideInInspector]
    public int currentIndex = 0;
    
    [System.Serializable]
    public class EnvironmentSound
    {
        public string soundName;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        public float playInterval = 0f;
        
        [HideInInspector]
        public AudioSource source;
        [HideInInspector]
        public bool isPlaying;
        [HideInInspector]
        public float nextPlayTime;
    }
    
    public EnvironmentSound GetNextSound()
    {
        if (sounds.Count == 0) return null;
        
        EnvironmentSound selectedSound;
        if (playSequentially)
        {
            selectedSound = sounds[currentIndex];
            currentIndex = (currentIndex + 1) % sounds.Count;
        }
        else
        {
            selectedSound = sounds[Random.Range(0, sounds.Count)];
        }
        
        return selectedSound;
    }
    
    public EnvironmentSound GetSpecificSound(string soundName)
    {
        return sounds.Find(s => s.soundName == soundName);
    }
} 