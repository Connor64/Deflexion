using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public static bool gamePaused = false;
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;
    public bool hasSettingsOpened = false;

    void Update() {
        if (gamePaused && AudioListener.volume != 0.5f) {
            AudioListener.volume = 0.5f;
        } else if (!gamePaused && AudioListener.volume != 1.0f) {
            AudioListener.volume = 1.0f;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (gamePaused && gameObject.GetComponent<SettingsMenu>().isScreenTransitioning() && gameObject.GetComponent<SettingsMenu>().getScreenBuffer() > 20) {
                // if the game is paused, it is "transitioning" between settings -> pause, and the time after going back to the pause menu from the settings menu is less than 20 ticks

                // checks if the game is paused, whether or not the settings menu is open, and if the time of the transition
                // between the settings and pause menu is greater than 50 frames (may not be frames, however many times it 
                // iterates)
                resume();
                gameObject.GetComponent<SettingsMenu>().setScreenTransition(false);
                gameObject.GetComponent<SettingsMenu>().setScreenBuffer(0);
                hasSettingsOpened = false;
            } else if (gamePaused && !hasSettingsOpened) {
                // if the game is paused and the settins haven't been opened at all, resume the game
                resume();
            }
            else {
                pause();
            }
        }
    }

    public void resume() {
        pauseMenuUI.SetActive(false);
        // disables the pause menu
        settingsMenuUI.SetActive(false);
        // disables the settings menu
        Time.timeScale = 1.0f;
        // makes the game run again
        gamePaused = false;
        // flags the game as not paused
    }

    void pause() {
        pauseMenuUI.SetActive(true);
        // enables the pause menu
        Time.timeScale = 0.0f;
        // stops the time of the game
        gamePaused = true;
        // flags the game as paused
    }

    public void loadSettings() {
        AudioManager.instance.play("Menu_selection");
        // plays a noise to indicate a selection
        hasSettingsOpened = true;
        // this is set to true once and is never turned back; it basically checks if the settings menu has been turned on at all
        gameObject.GetComponent<SettingsMenu>().setScreenOpen(true);
        // settings screen has been opened
        gameObject.GetComponent<SettingsMenu>().setScreenTransition(false);
        // it is no longer in the "transition" between settings -> pause
        gameObject.GetComponent<SettingsMenu>().setScreenBuffer(0);
        // sets the delay between pauses to 0 to reset the buffer
        print("screen open is true");
        pauseMenuUI.SetActive(false);
        // disables the pause menu
        settingsMenuUI.SetActive(true);
        // enables the settings menu
    }

    public void quitGame() {
        gamePaused = false;
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainMenu");
    }
}