using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserMovement : MonoBehaviour {
    public float laserVelocity = 20.0f;
    public GameObject player;
    public Camera cam;
    public GameObject particles;
    public GameObject destroyedPlayer;
    private Rigidbody2D rigidBody;

    private bool killedPlayer = false;
    private Vector3 killedPlayerPosition;

    public Vector2 origin;
    private Vector2 targetPosition;
    private Vector2 normalizeDirection;
    private Vector2 reversedDirection;

    public bool bounce = false;
    public bool homingLaser;
    private float xVel, yVel;

    [HideInInspector]
    public Shield playerShield;

    void Start() {
        rigidBody = GetComponent<Rigidbody2D>();
        transform.position = origin;
        targetPosition = player.transform.position;
        normalizeDirection = (targetPosition - origin).normalized;
        looksAtTarget();

        playerShield = player.GetComponentInChildren<Shield>();
        //reversedNormalizedDirection = (origin - targetPosition).normalized;
    }

    void FixedUpdate() {
        if (Screen.safeArea.Contains(cam.WorldToScreenPoint(transform.position))) {
            if (!bounce) {
                if (homingLaser) {
                    looksAtTarget();
                    rigidBody.MovePosition(Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), new Vector2(player.transform.position.x, player.transform.position.y), Time.fixedDeltaTime));
                } else {
                    rigidBody.MovePosition(new Vector2(transform.position.x, transform.position.y) + (normalizeDirection * laserVelocity * Time.fixedDeltaTime));
                }
            } else {
                rigidBody.MovePosition(new Vector2(transform.position.x, transform.position.y) + (reversedDirection.normalized * laserVelocity * Time.fixedDeltaTime));
            }
        } else {
            Destroy(gameObject);
        }
    }

    void calculateSlope() {
        float xDiff = targetPosition.x - origin.x;
        float yDiff = targetPosition.y - origin.y;
        float slope = xDiff / yDiff;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        GameObject collidedGO = collision.gameObject;
        if (collidedGO.name == "Shield" && !bounce) {
            switch (playerShield.shieldType) {
                case ShieldType.Deflector:
                    bounce = true;
                    reversedDirection = Vector2.Reflect(calculateVelocity(), collision.contacts[0].normal);
                    gameObject.layer = 14;
                    //looksFromTarget();
                    break;
                case ShieldType.Absorber:
                    playerShield.absorbLaser();
                    Destroy(gameObject);
                    break;
                case ShieldType.Basher:
                    bounce = true;
                    reversedDirection = Vector2.Reflect(calculateVelocity(), collision.contacts[0].normal);
                    gameObject.layer = 14;

                    // same as deflector until it's implemented
                    break;
                default:
                    Debug.LogError("Unable to detect shield type");
                    break;
            }
        }
        if (collision.gameObject.tag == "PlayerObject" && !killedPlayer) {
            GameObject playerGoBoom = Instantiate(destroyedPlayer);
            playerGoBoom.transform.position = collision.gameObject.transform.position;
            killedPlayerPosition = playerGoBoom.transform.position;
            playerGoBoom.GetComponentInChildren<PlayerExplosion>().explode = true;
            killedPlayer = true;
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "CamBounds") {
            Destroy(gameObject);
        }
    }

    private Vector2 calculateVelocity() {
        if (transform.localEulerAngles.z > 0.0f) {
            xVel = Mathf.Cos((transform.localEulerAngles.z - 90.0f) * (3.14f / 180)) * laserVelocity;
            yVel = Mathf.Sin((transform.localEulerAngles.z - 90.0f) * (3.14f / 180)) * laserVelocity;
        } else {
            xVel = Mathf.Cos((transform.localEulerAngles.z + 90.0f) * (3.14f / 180)) * laserVelocity;
            yVel = Mathf.Sin((transform.localEulerAngles.z + 90.0f) * (3.14f / 180)) * laserVelocity;
        }
        return new Vector2(xVel, yVel);
    }

    void looksAtTarget() {
        //Vector3 gohere = Camera.main.ScreenToWorldPoint(player.transform.position);

        /*Quaternion rot = Quaternion.LookRotation(transform.position - targetPosition, Vector3.forward);
        transform.rotation = rot;
        print(transform.localEulerAngles.z);
        transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z);
        */
        float angle = Mathf.Atan2((player.transform.position - transform.position).y, (player.transform.position - transform.position).x);
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, (angle * Mathf.Rad2Deg) + 90);
        particles.GetComponent<ParticleSystem>().shape.rotation.Set(particles.GetComponent<ParticleSystem>().shape.rotation.x, transform.rotation.y, particles.GetComponent<ParticleSystem>().shape.rotation.z);

        //transform.LookAt(player.transform);
        //transform.rotation = Quaternion.Euler(0.0f, 0.0f, transform.rotation.eulerAngles.z);

        //Quaternion lookRotation = Quaternion.LookRotation(player.transform.position, Vector3.up);
        //transform.rotation = Quaternion.Euler(0.0f, 0.0f, lookRotation.eulerAngles.z);
    }
    void looksFromTarget() {
        float angle = Mathf.Atan2((player.transform.position - transform.position).y, (player.transform.position - transform.position).x);
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, (angle * Mathf.Rad2Deg) - 90);
        // not necessary as bullets won't have a trail
    }
}