using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public enum ShieldType {
    Deflector,
    Absorber,
    Basher
};

public class Shield : MonoBehaviour {
    private Vector3 mousePosition;
    private Vector3 origin = new Vector3(0, 0, 0);
    private float thumbstickAngle;
    public GameObject player;
    
    private int lasersAbsorbed;
    // Only if the shield is an absorber shield

    public Sprite deflectorSprite;
    public Sprite[] absorberSprites;
    public Sprite basherSprite;

    private SpriteRenderer spriteRenderer;

    private bool enemyLockOn = false;

    public static bool arrowKeyRotate = false;
    public static bool controllerUsage = false;

    public ShieldType shieldType;

    void Start() {
        mousePosition = origin;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        switch(shieldType) {
            case ShieldType.Deflector:
                spriteRenderer.sprite = deflectorSprite;
                break;
            case ShieldType.Absorber:
                spriteRenderer.sprite = absorberSprites[0];
                break;
            case ShieldType.Basher:
                spriteRenderer.sprite = basherSprite;
                break;
        }
    }

    void Update() {
        if (!PauseMenu.gamePaused) {
            // if the game is not paused
            if (arrowKeyRotate) {
                arrowKeySnap();
                // use the arrow keys to rotate the shield around the player
            } else {
                if (!enemyLockOn) {
                    lookAt(Input.mousePosition);
                    // use the mouse to rotate the shield around the player
                }
            }
        }
    }

    public void lookAt(Vector3 position) {
        if (enemyLockOn) {
            float angle;

            float _vertical = position.y > transform.position.y ? Mathf.Abs(position.y - transform.position.y) : Mathf.Abs(transform.position.y - position.y);
            bool above = position.y > transform.position.y;
            // calculates the vertical displacement between the player and targeted enemy

            float _horizontal = position.x > transform.position.x ? Mathf.Abs(position.x - transform.position.x) : Mathf.Abs(transform.position.x - position.x);
            bool toRight = position.x > transform.position.x;
            // calculates the horizontal displacement between the player and targeted enemy

            float _hypotenuse = Mathf.Sqrt((_vertical * _vertical) + (_horizontal * _horizontal));
            // calculates the hypotenuse between the player and targeted enemy based on the vertical and horizontal displacements previously calculated

            if (_vertical > _horizontal) {
                angle = Mathf.Asin(_horizontal / _hypotenuse) * (180 / Mathf.PI);
            } else {
                angle = Mathf.Asin(_vertical / _hypotenuse) * (180 / Mathf.PI);
            }
            // the angle is calculated based on whether or not the vertical displacement vector is larger than the horizontal
            // pseudo-code: if vertical is larger -> use the horizontal displacement in equation, otherwise use the vertical displacement

            // this corrects the angle at which the shield rotates due to how the angle is initially calculated (I have to tell the code which
            // portions of which quadrant the enemy is in so that it rotates accordingly).
            // origin is the player's position
            if (above && toRight) {
                // 1st quadrant
                if (_horizontal > _vertical) {
                    angle -= 90;
                } else {
                    angle -= angle * 2;
                }
            } else if (above && !toRight) {
                // 2nd quadrant
                if (_horizontal > _vertical) {
                    angle = 90 - angle;
                }
            } else if (!above && !toRight) {
                // third quadrant
                if (_horizontal > _vertical) {
                    angle += 90;
                } else {
                    angle += (90 - angle) * 2;
                }
            } else if (!above && toRight) {
                // fourth quadrant
                if (_horizontal > _vertical) {
                    angle -= angle * 2;
                    angle -= 90;
                } else {
                    angle -= 180;
                }
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), 10 * Time.deltaTime);
        } else {
            position.x = (position.x / Screen.width) * 512;
            position.y = (position.y / Screen.height) * 288;
            // due to pixel perfect-ification, the mouse position gets all whack so I have to correct it
            // divide position components by screen width and height -> multiply by intended width and height respectively

            mousePosition = position;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.position - mousePosition, Vector3.forward), 10 * Time.deltaTime);
            // smoothly interpolates between mouse position and current rotation. (maybe incorporate that into transitioning from target to mouse position
            // so it's not so snappy?
            Quaternion targetRotation = Quaternion.Euler(0.0f, 0.0f, Quaternion.LookRotation(transform.position - mousePosition, Vector3.forward).eulerAngles.z);
            transform.rotation = targetRotation;
        }
    }

    void arrowKeySnap() {
        // the angles are measured from the "up-horizon"

        if (Input.GetKey(KeyCode.UpArrow)) {
            if (Input.GetKey(KeyCode.LeftArrow)) {
                transform.rotation = Quaternion.Euler(0, 0, 45);
                // if the UP and LEFT arrow keys are pressed, set the shield to a 45 degree angle relative to the player
            } else if (Input.GetKey(KeyCode.RightArrow)) {
                transform.rotation = Quaternion.Euler(0, 0, 315);
                // if the UP and RIGHT arrow keys are pressed, set the shield to a 315 degree angle relative to the player
            } else {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                // if the UP arrow key is pressed, set the shield to a 0 degree angle relative to the player
            }
        } else if (Input.GetKey(KeyCode.DownArrow)) {
            if (Input.GetKey(KeyCode.LeftArrow)) {
                transform.rotation = Quaternion.Euler(0, 0, 135);
                // if the DOWN and LEFT arrow keys are pressed, set the shield to a 135 degree angle relative to the player
            } else if (Input.GetKey(KeyCode.RightArrow)) {
                transform.rotation = Quaternion.Euler(0, 0, 215);
                // if the DOWN and RIGHT arrow keys are pressed, set the shield to a 215 degree angle relative to the player
            } else {
                transform.rotation = Quaternion.Euler(0, 0, 180);
                // if the DOWN arrow key is pressed, set the shield to a 180 degree angle relative to the player
            }
        } else if (Input.GetKey(KeyCode.LeftArrow)) {
            transform.rotation = Quaternion.Euler(0, 0, 90);
            // if the LEFT arrow key is pressed, set the shield to a 90 degree angle relative to the player
        } else if (Input.GetKey(KeyCode.RightArrow)) {
            transform.rotation = Quaternion.Euler(0, 0, -90);
            // if the RIGHT arrow key is pressed, set the shield to a -90 degree angle (
        }

    }

    public void absorbLaser() {
        lasersAbsorbed++;
        spriteRenderer.sprite = absorberSprites[lasersAbsorbed];
    }

    void thumbstickRotate() {
        //thumbstickAngle = Mathf.Sqrt()
        transform.rotation = Quaternion.Euler(0, 0, thumbstickAngle);
    }

    public void setLockedOn(bool a) {
        enemyLockOn = a;
    }

    public bool isLockedOn() {
        return enemyLockOn;
    }
}