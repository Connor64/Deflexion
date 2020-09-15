using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour {

    public GameObject[] backgrounds;
    private Vector3 newPositions;
    private Vector3 origin = new Vector3(0, 0, -20.0f);
    private float cameraHeight = 200.0f;
    private int currentScreen = 0;

    private void Start() {
/*        int y = 0;
        for(int i = 0; i < backgrounds.Length; i++) {
            origin = origin + new Vector3(y, 0, 0);
            backgrounds[i].transform.position = origin;
            y += 100;
        }   
        */
    }

    void Update() {
        newPositions = new Vector3(0, .01f, 0);
        transform.position = transform.position + newPositions;
        cameraHeight = cameraHeight - transform.position.y;
        if (cameraHeight <= 100.0f) {
            cameraHeight = 200 + transform.position.y;
            backgrounds[currentScreen].transform.position = transform.position + new Vector3(0, 2.0f, 0);
            if (currentScreen < backgrounds.Length - 1) {
                currentScreen += 1;
            } else if (currentScreen == backgrounds.Length - 1) {
                currentScreen = 0;
            }
        }
    }
}