using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine;
using System;
using System.Security.Cryptography;
using Microsoft.Win32.SafeHandles;

public class CameraEffects : MonoBehaviour {
    public static CameraEffects instance;
    public Camera cam;
    public GameObject player;
    private float leftBound, rightBound, upperBound, lowerBound;

    private float speedLineSpawnThreshold = 0;
    private float speedLineSpawnSeed;
    private float alpha = 0.0f;
    public GameObject speedLine;

    private bool speedLines = false;
    private bool zoomingIn = false;
    private bool zoomingOut = false;

    public GameObject postProcessing;
    private Vignette vg;
    private ChromaticAberration chromatic;
    private UnityEngine.Rendering.VolumeProfile volumeProfile;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
    }

    void Start() {
        volumeProfile = postProcessing.GetComponent<UnityEngine.Rendering.Volume>().profile;
        volumeProfile.TryGet(out vg);
        volumeProfile.TryGet(out chromatic);
    }

    void Update() {
        if (!zoomingIn && !zoomingOut) {
            transform.position = new Vector3(0, 0, -10);
        }
        checkBounds();
        if (!inBounds()) {
            correctBounds();
        }
    }

    public void checkBounds() {
        leftBound = transform.position.x - ((cam.orthographicSize / 9.0f) * 16.0f);
        rightBound = transform.position.x + ((cam.orthographicSize / 9.0f) * 16.0f);
        upperBound = transform.position.y + cam.orthographicSize;
        lowerBound = transform.position.y - cam.orthographicSize;
        // checks where each boundary is located
    }

    public void correctBounds() {
        if (!inLeftBound()) {
            print("LEFT correction");
            transform.position = transform.position - new Vector3((leftBound + 16), 0, 0);
        } else if (!inRightBound()) {
            //print("RIGHT correction");
            transform.position = transform.position - new Vector3((rightBound - 16), 0, 0);
        }
        if (!inUpperBound()) {
            //print("UPPER correction");
            transform.position = transform.position - new Vector3(0, (upperBound - 9), 0);
        } else if (!inLowerBound()) {
            //print("LOWER correction");
            transform.position = transform.position - new Vector3(0, (lowerBound + 9), 0);
        }
        // if the camera goes out of the bounds of the level, the camera is pushed to be flush with the level so that outside of the level is never shown
    }
    
    public bool inBoundsExclusive() {
        return ((leftBound > -16) && (rightBound < 16) && (upperBound < 9) && (lowerBound > -9));
    }

    public bool inBounds() {
        return (inLeftBound() && inRightBound() && inUpperBound() && inLowerBound());
    }

    public bool inLeftBound() {
        return (leftBound >= -16);
    }

    public bool inRightBound() {
        return (rightBound <= 16);
    }

    public bool inUpperBound() {
        return (upperBound <= 9);
    }

    public bool inLowerBound() {
        return (lowerBound >= -9);
    }

    public void zoomIn(Vector3 position, float zoomScale, float speed) {
        checkBounds();
        if (cam.orthographicSize > zoomScale) {
            cam.orthographicSize = cam.orthographicSize - speed * Time.deltaTime;
            zoomingIn = true;
            zoomingOut = false;
            //GetComponent<BoxCollider2D>().size = new Vector3((cam.orthographicSize / 9) * 32, cam.orthographicSize * 2);
        }
        if (cam.orthographicSize < zoomScale) {
            cam.orthographicSize = zoomScale;
            //GetComponent<BoxCollider2D>().size = new Vector3((cam.orthographicSize / 9) * 32, cam.orthographicSize * 2);
        }
        checkBounds();
        if (inBounds()) {
            transform.position = Vector3.Lerp(transform.position, new Vector3(position.x, position.y, transform.position.z), Time.deltaTime * 5.5f);
        }
        checkBounds();
        if (leftBound < -16) {
            transform.position = new Vector3(transform.position.x - (leftBound + 16), transform.position.y, transform.position.z);
        }
        checkBounds();
        if (rightBound > 16) {
            transform.position = new Vector3(transform.position.x - (rightBound - 16), transform.position.y, transform.position.z);
        }
        checkBounds();
        if (upperBound > 9) {
            transform.position = new Vector3(transform.position.x, transform.position.y - (upperBound - 9), transform.position.z);
        }
        checkBounds();
        if (lowerBound < -9) {
            transform.position = new Vector3(transform.position.x, transform.position.y + (lowerBound + 9), transform.position.z);
        }
        vignetteEffect(true);
        chromaticAberration(true, false);
    }

    public void zoomOut(float speed) {
        if (cam.orthographicSize < 9.0f) {
            cam.orthographicSize = cam.orthographicSize + 9.0f * Time.deltaTime * .75f;
            zoomingIn = false;
            zoomingOut = true;
        }
        if (cam.orthographicSize > 9.0f) {
            cam.orthographicSize = 9.0f;
            zoomingOut = false;
        }
        if (inBoundsExclusive()) {
            transform.position = Vector3.Lerp(transform.position, new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z), Time.deltaTime * 1.5f);
        }
        checkBounds();
        if (leftBound < -16) {
            transform.position = new Vector3(transform.position.x - (leftBound + 16), transform.position.y, transform.position.z);
        }
        checkBounds();
        if (rightBound > 16) {
            transform.position = new Vector3(transform.position.x - (rightBound - 16), transform.position.y, transform.position.z);
        }
        checkBounds();
        if (upperBound > 9) {
            transform.position = new Vector3(transform.position.x, transform.position.y - (upperBound - 9), transform.position.z);
        }
        checkBounds();
        if (lowerBound < -9) {
            transform.position = new Vector3(transform.position.x, transform.position.y + (lowerBound + 9), transform.position.z);
        }
        vignetteEffect(false);
        chromaticAberration(false, false);
        AudioManager.instance.resetPitch();
    }

    public void vignetteEffect(bool enable) {
        if (enable) {
            // print(vg.intensity.value);
            if (vg.intensity.value < 0.4f) {
                vg.intensity.Override(Mathf.Lerp(vg.intensity.value, 0.4f, 2.5f * Time.unscaledDeltaTime));
            }
            if (vg.intensity.value < 0.4f && vg.intensity.value > 0.39f) {
                vg.intensity.Override(0.4f);
            }
        } else {
            if (vg.intensity.value > 0.15f) {
                vg.intensity.Override(Mathf.Lerp(vg.intensity.value, 0.15f, 2.0f * Time.unscaledDeltaTime));
            }
            if (vg.intensity.value > 0.15f && vg.intensity.value < 0.16f) {
                vg.intensity.Override(0.15f);
            }
        }
    }

    public void chromaticAberration(bool enable, bool glitch) {
        if (enable) {
            if (chromatic.intensity.value < 0.15f) {
                chromatic.intensity.Override(Mathf.Lerp(chromatic.intensity.value, 0.15f, 2.5f * Time.unscaledDeltaTime));
            }
            if (chromatic.intensity.value < 0.15f) {
                chromatic.intensity.Override(0.15f);
            }
        } else {
            if (chromatic.intensity.value > 0) {
                chromatic.intensity.Override(Mathf.Lerp(chromatic.intensity.value, 0.0f, 2.5f * Time.unscaledDeltaTime));
            }
            if (chromatic.intensity.value > 0.0f && chromatic.intensity.value < 0.01f) {
                chromatic.intensity.Override(0);
            }
        }
    }

    public void speedLineSpawn(bool enable) {
        speedLines = enable;
        if (enable) {
            speedLineSpawnSeed = UnityEngine.Random.Range(0.0f, 15.0f);
            //print("SEED: " + speedLineSpawnSeed);
            //print("THRESHOLD: " + speedLineSpawnThreshold);
            if (alpha < 1) {
                alpha = alpha + 2.5f * Time.deltaTime;
            }
            if (alpha > 1) {
                alpha = 1.0f;
            }

            if (speedLineSpawnSeed >= 0 && speedLineSpawnSeed <= speedLineSpawnThreshold) {
                var speedLineThing = GameObject.Instantiate(speedLine).GetComponent<SpeedLine>();
                speedLineThing.Initialize(alpha);
                //print("dude a line WOAH");
            }

            if (speedLineSpawnThreshold < 10) {
                speedLineSpawnThreshold = speedLineSpawnThreshold + 1.5f * Time.deltaTime;
            }
            if (speedLineSpawnThreshold > 10) {
                speedLineSpawnThreshold = 10.0f;
            }
        } else {
            speedLineSpawnSeed = UnityEngine.Random.Range(0.0f, 15.0f);
            if (alpha > 0) {
                alpha = alpha - 1.05f * Time.deltaTime;
            }
            if (alpha < 0) {
                alpha = 0.0f;
            }
            if (speedLineSpawnSeed >= 0 && speedLineSpawnSeed <= speedLineSpawnThreshold && speedLineSpawnThreshold != 0.0f) {
                var speedLineThing = GameObject.Instantiate(speedLine).GetComponent<SpeedLine>();
                speedLineThing.Initialize(alpha);
                //print("dude a line WOAH");
            }
            if (speedLineSpawnThreshold > 0) {
                speedLineSpawnThreshold = speedLineSpawnThreshold - 5.5f * Time.deltaTime;
            }
            if (speedLineSpawnThreshold < 0) {
                speedLineSpawnThreshold = 0.0f;
            }
        }
    }

    public bool areSpeedLines() {
        return speedLines;
    }

}
