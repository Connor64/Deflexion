using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ShipMovement : MonoBehaviour {
    private Vector3 position;
    public float flySpeed = 10;
    public Camera cam;
    public GameObject backgrounds;
    public Animator animator;
    private Rigidbody2D rigidBody;
    public float linearDrag = 40.0f;

    private bool boosting = false;
    private bool boostLock = false;
    private bool inSlowMotion;
    private Vector2 currentVelocity;
    public float speedLimit = 15.0f;
    public float acceleration = 300.0f;
    private float squareSpeedLimit;
    private float squareBoostLimit;

    private ParallaxScroll parallaxInstance;

    void Awake() {
        rigidBody = GetComponent<Rigidbody2D>();
        squareSpeedLimit = speedLimit * speedLimit;
        squareBoostLimit = (speedLimit / 5) * (speedLimit / 5);
        rigidBody.drag = linearDrag;

        parallaxInstance = ParallaxScroll.instance;
    }

    void FixedUpdate() {
        if (!PauseMenu.gamePaused) {
            floatyControls();
            if (!inSlowMotion) {
                playerBoost();
            }
            boosting = ParallaxScroll.instance.speedUp;
            shipAnimation();
            animationCheck();
            slowMotion();
            //crushDetection();
        }
    }

    public void floatyControls() {
        if (acceleration < 50.0f) {
            acceleration += 1.5f * Time.deltaTime;
            if (acceleration > 50.0f) {
                acceleration = 50.0f;
            }
        }
        // this is used for when you come back from boosting to return the acceleration back to the normal speed (50.0f).
        // basically simulates slowing back down
        if (!boosting) {
            position = new Vector2(Input.GetAxis("Horizontal") * acceleration, Input.GetAxis("Vertical") * acceleration);
            // creates a vector of the velocity
            rigidBody.AddForce(position);
            currentVelocity = rigidBody.velocity;
            if (currentVelocity.sqrMagnitude > squareSpeedLimit) {
                rigidBody.velocity = currentVelocity.normalized * speedLimit;
            }
            // keeps the player speed from exceeding the speed limit
        } else if (boosting) {
            if (acceleration > 50.0f) {
                acceleration -= 1.5f * Time.deltaTime;
                if (acceleration < 50.0f) {
                    acceleration = 50.0f;
                }
            }
            // decreases the acceleration until it reaches the normal speed (50.0f).
            position = new Vector2(Input.GetAxis("Horizontal") * acceleration, -acceleration / 2);
            rigidBody.AddForce(position);
            currentVelocity = rigidBody.velocity;
            if (currentVelocity.sqrMagnitude > squareBoostLimit) {
                rigidBody.velocity = currentVelocity.normalized * (speedLimit / 5);
            }
        }
    }

    public void slowMotion() {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) && ParallaxScroll.instance.parallaxScrollSpeed == 1) {
            ParallaxScroll.instance.slowDown = true;
            ParallaxScroll.instance.speedUp = false;
            AudioManager.instance.lowPass(true);
            Time.timeScale = 0.3f;
            Time.fixedDeltaTime = 0.0048f;
            inSlowMotion = true;
            CameraEffects.instance.zoomIn(transform.position, 7.5f, 10.0f);
        } else {
            CameraEffects.instance.zoomOut(.75f);
            if (Time.timeScale != 1) {
                inSlowMotion = false;
                AudioManager.instance.lowPass(false);
                Time.timeScale = 1.0f;
                Time.fixedDeltaTime = 0.02f;
            }
        }
    }

    public void playerBoost() {
        if (!Input.GetKey(KeyCode.Space)) {
            boostLock = false;
        }
        if (!boosting) {
            AudioManager.instance.lowPass(false);
            CameraEffects.instance.vignetteEffect(false);
            // make sure that the lowpass filter is always off if the ship is not boosting
        }
        if (boostLock && Input.GetKeyUp(KeyCode.Space)) {
            boostLock = false;
        }
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.Space) && !inSlowMotion && !boostLock) {
            CameraEffects.instance.speedLineSpawn(true);
            if (transform.position.y > -9.0f) {
                // if the ship is not outside of the max screen area
                AudioManager.instance.lowPass(true);
                ParallaxScroll.instance.speedUp = true;
                CameraEffects.instance.vignetteEffect(true);
                // move player slowly back, enable the speed-up of the parallax scroll to simulate boosting, and enable the lowpass filter
            } else {
                AudioManager.instance.lowPass(false);
                ParallaxScroll.instance.speedUp = false;
                CameraEffects.instance.vignetteEffect(false);
                CameraEffects.instance.speedLineSpawn(false);
            }
        }
        if ((!Input.GetKey(KeyCode.W) || !Input.GetKey(KeyCode.Space)) && !inSlowMotion || transform.position.y < -8.5f) {
            AudioManager.instance.lowPass(false);
            CameraEffects.instance.vignetteEffect(false);
            CameraEffects.instance.speedLineSpawn(false);
            ParallaxScroll.instance.speedUp = false;
            ParallaxScroll.instance.slowDown = true;
            // if either W or the space button aren't pressed, disable boosting and the lowpass filter
        }
        if (transform.position.y < -8.5f) {
            boostLock = true;
        }
    }
    public void shipAnimation() {
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) {
            animator.SetBool("turnLeft", true);
            animator.SetBool("right", animator.GetCurrentAnimatorStateInfo(0).IsName("Player_idle_right"));
            animator.SetBool("center", animator.GetCurrentAnimatorStateInfo(0).IsName("Player_idle_normal"));
            animator.SetBool("left", animator.GetCurrentAnimatorStateInfo(0).IsName("Player_idle_left"));
        } else {
            animator.SetBool("turnLeft", false);
        }
        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) {
            animator.SetBool("turnRight", true);
            animator.SetBool("right", animator.GetCurrentAnimatorStateInfo(0).IsName("Player_idle_right"));
            animator.SetBool("center", animator.GetCurrentAnimatorStateInfo(0).IsName("Player_idle_normal"));
            animator.SetBool("left", animator.GetCurrentAnimatorStateInfo(0).IsName("Player_idle_left"));
        } else {
            animator.SetBool("turnRight", false);
        }
    }
    public void animationCheck() {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Player_idle_left") && !Input.GetKey(KeyCode.A)) {
            animator.SetBool("left", true);
            animator.SetBool("right", false);
            animator.SetBool("center", false);
            animator.SetBool("turnLeft", false);
            animator.SetBool("turnRight", false);
        } else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Player_idle_right") && !Input.GetKey(KeyCode.D)) {
            animator.SetBool("left", false);
            animator.SetBool("right", true);
            animator.SetBool("center", false);
            animator.SetBool("turnLeft", false);
            animator.SetBool("turnRight", false);
        } else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Player_idle_left") && Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A)) {
            animator.SetBool("left", true);
            animator.SetBool("right", false);
            animator.SetBool("center", false);
            animator.SetBool("turnLeft", false);
            animator.SetBool("turnRight", false);
        } else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Player_idle_right") && Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A)) {
            animator.SetBool("left", false);
            animator.SetBool("right", true);
            animator.SetBool("center", false);
            animator.SetBool("turnLeft", false);
            animator.SetBool("turnRight", false);
        }
    }
}