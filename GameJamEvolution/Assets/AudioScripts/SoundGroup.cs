using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Sound Group", menuName = "Audio/Sound Group")]
public class SoundGroup : ScriptableObject
{
    public string groupName;
    public List<Sound> sounds = new List<Sound>();
    public bool playSequentially = false;
    
    [HideInInspector]
    public int currentIndex = 0;
    
    public Sound GetNextSound()
    {
        if (sounds.Count == 0) return null;
        
        Sound selectedSound;
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
    
    public Sound GetSpecificSound(string soundName)
    {
        return sounds.Find(s => s.name == soundName);
    }
} 