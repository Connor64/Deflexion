using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyAI : MonoBehaviour {

    public Camera cam;
    public GameObject tileset;
    public GameObject laser;
    public GameObject player;

    private Rigidbody2D rigidBody;

    public float enemySpeed = 2.0f;

    private bool turningRight = false;
    private bool turningLeft = false;
    private bool inRange = false;
    [SerializeField]
    private float cooldown = 0;
    private bool cooldownCounter = false;
    public int cooldownTimer = 10;

    [HideInInspector]
    public LaserMovement laserMovement;
    public Shield playerShield;

    private void Start() {
        laser.GetComponent<LaserMovement>().player = player;
        laser.GetComponent<LaserMovement>().cam = cam;
        rigidBody = GetComponent<Rigidbody2D>();

        rigidBody.MovePosition(new Vector2(UnityEngine.Random.Range(-14, 14), 12));

        playerShield = player.GetComponentInChildren<Shield>();
        laserMovement = laser.GetComponent<LaserMovement>();
    }

    void FixedUpdate() {
        // inBounds(transform.localPosition);

        fireLaser();
        move();
    }

    public void inBounds(Vector3 screenPosition) {
        if (screenPosition.y < 0) {
            if (!Screen.safeArea.Contains(new Vector3(0, screenPosition.y + 3, 0))) {
                Destroy(gameObject);
                print("Destroyed " +gameObject.name);
            }
        }
    }

    public void fireLaser() {
        if (inRange && cooldown == 0) {
            laserMovement.origin = new Vector2(transform.position.x, transform.position.y);
            Instantiate(laser);
            cooldownCounter = true;
        }
        if (cooldownCounter) {
            cooldown += 1 * Time.deltaTime;
            if (cooldown >= cooldownTimer) {
                cooldown = 0;
                cooldownCounter = false;
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == 14 && collision.gameObject.GetComponent<LaserMovement>().bounce) {
            playerShield.setLockedOn(false);
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        inRange = (collision.gameObject.name == "EnemyDetector");
        if (collision.gameObject.tag == "KillPlane") {
            Destroy(gameObject);
            print("Destroyed " + gameObject.name);
        }
    }

    public void OnTriggerExit2D(Collider2D collision) {
        inRange = !(collision.gameObject.name == "EnemyDetector");
    }

    public void move() {
        rigidBody.MovePosition(transform.position + (transform.up * Time.fixedDeltaTime * -enemySpeed));
        //circleRight();
        circleLeft();
        // transform.Translate(transform.forward * enemySpeed * Time.deltaTime);
        
    }

    public void circleRight() {
        turningRight = true;
        turningLeft = false;
        transform.Rotate(transform.forward * Time.deltaTime * 60.0f);
    }

    public void circleLeft() {
        turningRight = false;
        turningLeft = true;
        transform.Rotate(transform.forward * Time.deltaTime * -60.0f);
    }

    public void flyDown() {
        rigidBody.MovePosition(Vector2.Lerp(transform.position, new Vector2(transform.position.x, UnityEngine.Random.Range(8, 6)), Time.fixedDeltaTime));
    }

    //private IEnumerator flyDown2() {
    //
    //}
}