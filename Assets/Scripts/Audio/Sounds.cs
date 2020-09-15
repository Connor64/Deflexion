using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sounds {

    public string name;
    public AudioClip clip;

    [Range(0.0f, 1.0f)]
    public float volume;

    [Range(0.0f, 3.0f)]
    public float pitch;

    [HideInInspector]
    public AudioSource source;

    [HideInInspector]
    public bool playing = false;

    public bool loop = false;

}