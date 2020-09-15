using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {
    public GameObject previousScreen;
    public GameObject settingsMenuUI;
    public GameObject arrowKeyRotateToggle;
    public GameObject controllerToggle;
    public GameObject screenDropDown;

    public GameObject controlsSubMenu;
    public GameObject audioSubMenu;
    public GameObject graphicsSubMenu;

    public TMP_Text[] controlsButtonText;
    public TMP_Text[] audioButtonText;
    public TMP_Text[] graphicsButtonText;

    //public GameObject screenToggle;
    public GameObject screenSlider;
    private Slider slider;
    private TMP_Dropdown dropdown;
    public bool screenOpen = false;
    public int screenBuffer = 0;
    public bool screenTransition = false;

    void Start() {
        //slider = screenSlider.GetComponent<Slider>();
        dropdown = screenDropDown.GetComponent<TMP_Dropdown>();
    }

    void Update() {
        if (arrowKeyRotateToggle.GetComponent<Toggle>().isOn != Shield.arrowKeyRotate) {
            arrowKeyRotateToggle.GetComponent<Toggle>().isOn = Shield.arrowKeyRotate;
        }
        //checkScreenSizeUpdatesViaSlider();
        checkScreenSizeUpdates();

        if (Input.GetKey(KeyCode.Escape) && screenOpen) {
            returnToPrevious();
            // will begin to count how long after the settings menu is closed
        }
        if (screenTransition) {
            screenBuffer++;
            //print(screenBuffer);
            // this is to ensure that the pause menu is still opened after the settings menu is closed
            // Previously, it jumped straight back to the game as it was instantaneous
        }

    }

    public void checkScreenSizeUpdatesViaSlider() {
        if (Screen.fullScreen && slider.value != 0.0f) {
            slider.SetValueWithoutNotify(0.0f);
        }
        if (Screen.width == 1536 && !Screen.fullScreen && slider.value != 1.0f) {
            slider.SetValueWithoutNotify(1.0f);
        }
        if (Screen.width == 1280 && !Screen.fullScreen && slider.value != 2.0f) {
            slider.SetValueWithoutNotify(2.0f);
        }
        if (Screen.width == 1024 && !Screen.fullScreen && slider.value != 3.0f) {
            slider.SetValueWithoutNotify(3.0f);
        }
        if (Screen.width == 512 && !Screen.fullScreen && slider.value != 4.0f) {
            print(slider.value);
            slider.SetValueWithoutNotify(4.0f);
        }
    }

    public void checkScreenSizeUpdates() {
        if (Screen.fullScreen && screenDropDown.GetComponent<TMP_Dropdown>().value != 0) {
            dropdown.SetValueWithoutNotify(0);
        }
        if (Screen.width == 1536 && screenDropDown.GetComponent<TMP_Dropdown>().value != 1) {
            dropdown.SetValueWithoutNotify(1);
        }
        if (Screen.width == 1280 && screenDropDown.GetComponent<TMP_Dropdown>().value != 2) {
            dropdown.SetValueWithoutNotify(2);
        }
        if (Screen.width == 1024 && screenDropDown.GetComponent<TMP_Dropdown>().value != 3) {
            dropdown.SetValueWithoutNotify(3);
        }
        if (Screen.width == 512 && screenDropDown.GetComponent<TMP_Dropdown>().value != 4) {
            dropdown.SetValueWithoutNotify(4);
        }
    }

    public void returnToPrevious() {
        // returns to the previous screen (not just the pause menu as this screen will be accessible from the main menu and the pause menu)
        AudioManager.instance.play("Menu_selection");
        // plays a noise to indicate a selection
        settingsMenuUI.SetActive(false);
        // disables the settings menu
        previousScreen.SetActive(true);
        if (previousScreen.name == "MainMenu") {
            MainMenu.instance.freeformLight.SetActive(true);
        }
        // enables whatever the previous screen was
        screenTransition = true;
        // flags the menus as transitioning between eachother
        screenOpen = false;
        // flags the settings menu as not open
    }

    public void setScreenSize(int size) {
        switch (size) {
            case 0:
                Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, FullScreenMode.MaximizedWindow, 60);
                print("FULLSCREEN");
                break;
            case 1:
                Screen.SetResolution(1536, 864, FullScreenMode.Windowed, 60);
                print("1536 x 864");
                break;
            case 2:
                //Screen.SetResolution(1920, 1080, false);
                Screen.SetResolution(1280, 720, FullScreenMode.Windowed, 60);
                print("1280 x 720");
                break;
            case 3:
                Screen.SetResolution(1024, 576, FullScreenMode.Windowed, 60);
                print("1024 x 576");
                break;
            case 4:
                Screen.SetResolution(512, 288, FullScreenMode.Windowed, 60);
                print("512 x 288");
                break;
            default:
                Screen.fullScreen = true;
                break;
        }
    }

    public void arrowKeyUsage(bool a) {
        AudioManager.instance.play("Menu_selection");
        // plays a noise to indicate a selection
        Shield.arrowKeyRotate = a;
        // sets the game to use arrow keys to rotate the shield
    }

    public void controllerUsage(bool a) {

    }

    public void openControls() {
        controlsSubMenu.SetActive(true);
        audioSubMenu.SetActive(false);
        graphicsSubMenu.SetActive(false);

        AudioManager.instance.play("Menu_selection");
        // plays a noise to indicate a selection

        foreach (TMP_Text t in controlsButtonText) {
            t.fontStyle = FontStyles.Underline;
        }
        foreach (TMP_Text t in audioButtonText) {
            t.fontStyle = FontStyles.Normal;
        }
        foreach (TMP_Text t in graphicsButtonText) {
            t.fontStyle = FontStyles.Normal;
        }
    }

    public void openAudio() {
        controlsSubMenu.SetActive(false);
        audioSubMenu.SetActive(true);
        graphicsSubMenu.SetActive(false);

        AudioManager.instance.play("Menu_selection");
        // plays a noise to indicate a selection

        foreach (TMP_Text t in controlsButtonText) {
            t.fontStyle = FontStyles.Normal;
        }
        foreach (TMP_Text t in audioButtonText) {
            t.fontStyle = FontStyles.Underline;
        }
        foreach (TMP_Text t in graphicsButtonText) {
            t.fontStyle = FontStyles.Normal;
        }
    }

    public void openGraphics() {
        controlsSubMenu.SetActive(false);
        audioSubMenu.SetActive(false);
        graphicsSubMenu.SetActive(true);

        AudioManager.instance.play("Menu_selection");
        // plays a noise to indicate a selection

        foreach (TMP_Text t in controlsButtonText) {
            t.fontStyle = FontStyles.Normal;
        }
        foreach (TMP_Text t in audioButtonText) {
            t.fontStyle = FontStyles.Normal;
        }
        foreach (TMP_Text t in graphicsButtonText) {
            t.fontStyle = FontStyles.Underline;
        }
    }

    public void setScreenTransition(bool a) {
        screenTransition = a;
    }

    public bool isScreenTransitioning() {
        return screenTransition;
    }

    public void setScreenBuffer(int a) {
        screenBuffer = a;
    }

    public int getScreenBuffer() {
        return screenBuffer;
    }

    public bool getScreenOpen() {
        return screenOpen;
    }

    public void setScreenOpen(bool a) {
        screenOpen = a;
    }
}