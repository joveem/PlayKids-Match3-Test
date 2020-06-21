using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Audio{
    
    public string name;
    public AudioType audio_type = AudioType.Sound;
    public AudioClip audio_clip;
    public bool play_on_awake = false, loop = false;
    [Range(0,1)]
    public float volume = 1;

    [Range(0,1)]
    public float spatial_blend = 0;
    [Range(-3,3)]
    public float pitch = 1;

    public AudioSource audio_source;

}

public enum AudioType{

    Music,
    Sound

}