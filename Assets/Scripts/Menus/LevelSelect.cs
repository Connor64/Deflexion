using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject levelSelect;

    public void loadLevel(string level) {
        AudioManager.instance.play("Menu_selection");
        // plays a noise to indicate a selection

        SceneManager.LoadScene(level);
    }

    public void backToMain() {
        levelSelect.SetActive(false);
        mainMenu.SetActive(true);
        MainMenu.instance.freeformLight.SetActive(true);
    }
}
