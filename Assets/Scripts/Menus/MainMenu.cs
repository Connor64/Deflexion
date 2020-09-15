using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;

public class MainMenu : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject levelSelect;
    public GameObject freeformLight;
    public GameObject currentBuild;
    public GameObject currentBuild2;

    public static MainMenu instance;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
        // Screen.fullScreen = true;
    }

    void Start() {
        currentBuild.GetComponent<TextMeshProUGUI>().SetText(Application.version);
        currentBuild2.GetComponent<TextMeshProUGUI>().SetText(Application.version);
    }

    public void loadSettings() {
        AudioManager.instance.play("Menu_selection");
        // plays a noise to indicate a selection

        mainMenu.SetActive(false);
        freeformLight.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void loadLevelSelect() {
        AudioManager.instance.play("Menu_selection");
        // plays a noise to indicate a selection

        mainMenu.SetActive(false);
        freeformLight.SetActive(false);
        levelSelect.SetActive(true);
    }

    public void quitGame() {
        Application.Quit();
    }
}