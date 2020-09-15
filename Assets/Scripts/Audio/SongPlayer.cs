using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SongPlayer : MonoBehaviour {
    private bool introStarted = false;
    private bool bodyStarted = true;
    private string currentLevel;

    void Start() {
        currentLevel = SceneManager.GetActiveScene().name;
    }
    void Update() {
        switch (currentLevel) {
            case "MainMenu":
            case "TestLevel":
                testLevelMusic2("Xyralon_intro", "Xyralon_body");
                break;
            default:
                Debug.LogError("SongPlayer: No Scene is Loaded??? WHAT?!?!?");
                break;
        }
    }

    void testLevelMusic2(string intro, string body) {
        if (!GetComponent<AudioManager>().isPlaying(intro) && !GetComponent<AudioManager>().isPlaying(body) && !introStarted && bodyStarted) {
            AudioManager.instance.play(intro);
            introStarted = true;
            bodyStarted = false;
        } else if (!GetComponent<AudioManager>().isPlaying(body) && !GetComponent<AudioManager>().isPlaying(intro) && introStarted && !bodyStarted) {
            AudioManager.instance.play(body);
        }
    }
}