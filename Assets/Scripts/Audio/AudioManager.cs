using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.Video;
using System;

public class AudioManager : MonoBehaviour {

    public GameObject cam;
    public Sounds[] sounds;
    public static AudioManager instance;
    private float lowPassSpeed = 50.0f;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
        // DontDestroyOnLoad(gameObject);
        // may not want this as there will be different music per level

        foreach (Sounds s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = false;
        }
    }

    void Start() {
        cam.AddComponent<AudioLowPassFilter>();
        cam.GetComponent<AudioLowPassFilter>().cutoffFrequency = 22000.0f;
    }

    public void lowPass(bool val) {
        if (!val) {
            if (cam.GetComponent<AudioLowPassFilter>().cutoffFrequency < 22000.0f) {
                cam.GetComponent<AudioLowPassFilter>().cutoffFrequency += 1000.0f * (lowPassSpeed/Time.timeScale) * Time.deltaTime;
            }
        } else {
            if (cam.GetComponent<AudioLowPassFilter>().cutoffFrequency > 5000.0f) {
                cam.GetComponent<AudioLowPassFilter>().cutoffFrequency -= 1000.0f * (lowPassSpeed / Time.timeScale) * Time.deltaTime;
            }
            if (cam.GetComponent<AudioLowPassFilter>().cutoffFrequency < 5000.0f) {
                cam.GetComponent<AudioLowPassFilter>().cutoffFrequency = 5000.0f;
            }
        }
    }

    public void play(string audioName) {
        Sounds s = Array.Find(sounds, sound => sound.name == audioName);
        if (s == null) {
            Debug.LogWarning("UNABLE TO FIND " + audioName);
            return;
        }
        s.source.Play();
        s.playing = true;
    }

    public void stopPlaying(string audioName) {
        Sounds s = Array.Find(sounds, sound => sound.name == audioName);
        if (s == null) {
            Debug.LogWarning("UNABLE TO FIND " + audioName);
            return;
        }
        s.source.Stop();
        s.playing = false;
    }

    public bool isPlaying(string audioName) {
        Sounds s = Array.Find(sounds, sound => sound.name == audioName);
        if (s == null) {
            Debug.LogWarning("UNABLE TO FIND " + audioName);
            return false;
        }
        return s.source.isPlaying;
    }

    public void turnDownAudio(string audioName, float volumePercentage) {
        Sounds s = Array.Find(sounds, sound => sound.name == audioName);
        if (s == null) {
            Debug.LogWarning("UNABLE TO FIND " + audioName);
            return;
        }
        s.source.volume = s.volume * volumePercentage;
    }

    public void turnUpAudio(string audioName) {
        Sounds s = Array.Find(sounds, sound => sound.name == audioName);
        if (s == null) {
            Debug.LogWarning("UNABLE TO FIND " + audioName);
            return;
        }
        s.source.volume = s.volume;
    }

    public void decreasePitch() {
        foreach (Sounds s in sounds) {
            s.source.pitch = Mathf.Lerp(s.source.pitch, 0.99f, 2.0f * Time.unscaledDeltaTime);
        }
    }

    public void resetPitch() {
        foreach (Sounds s in sounds) {
            s.source.pitch = Mathf.Lerp(s.source.pitch, 1.0f, 2.0f * Time.unscaledDeltaTime);
        }
    }

    public void increasePitch() {
        foreach (Sounds s in sounds) {
            s.source.pitch = Mathf.Lerp(s.source.pitch, 1.01f, 2.0f * Time.unscaledDeltaTime);
        }
    }

}