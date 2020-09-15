using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExplosion : MonoBehaviour {
    public bool explode = false;
    private int explosionCount = 0;
    private Vector3 zoomPosition;
    private bool deathSound = false;

    public void Initialize(Vector3 position) {
        transform.position = position;
    }

    void Start() {
        zoomPosition = transform.position;
    }

    void Update() {
        exploder();
    }

    public void exploder() {
        if (explode && explosionCount < 10) {
            GetComponent<PointEffector2D>().enabled = true;
            explosionCount++;
        } else {
            GetComponent<PointEffector2D>().enabled = false;
        }

        ParallaxScroll.instance.speedUp = false;
        ParallaxScroll.instance.slowDown = true;
        AudioManager.instance.lowPass(true);
        Time.timeScale = 0.3f;
        Time.fixedDeltaTime = 0.0048f;
        CameraEffects.instance.zoomIn(zoomPosition, 5.5f, 2.5f);
        if (!deathSound) {
            AudioManager.instance.play("Player_explosion");
            deathSound = true;
        }

    }
}
