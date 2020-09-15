using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;

public class PlayerCollision : MonoBehaviour {
    private bool tileCollision = false;
    private bool boundaryCollision = false;
    public GameObject player;

    private bool isUpper;

    public void Awake() {
        isUpper = gameObject.name == "UpperDetector";
    }

    void Update() {
            if (isUpper) {
            //player.GetComponent<ShipMovement>().tileCollision = tileCollision;
        } else {
            //player.GetComponent<ShipMovement>().boundaryCollision = boundaryCollision;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (isUpper) {
            tileCollision = collision.gameObject.tag == "Tiles";
        } else {
            boundaryCollision = collision.gameObject.tag == "PlayerBoundary";
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (isUpper) {
            tileCollision = !(collision.gameObject.tag == "Tiles");
        } else {
            boundaryCollision = !(collision.gameObject.tag == "PlayerBoundary");
        }
        // this method ensures that the collision is reset when it is not touching anything
    }
}