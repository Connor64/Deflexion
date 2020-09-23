using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlast : MonoBehaviour {

    public Vector3[] startingPositions = new Vector3[] {
        new Vector3(-1, 1, 0),
        new Vector3(0, 1, 0),
        new Vector3(1, 1, 0)
    };

    private Quaternion orientation;
    private Rigidbody rb;

    public float acceleration;
    public float speedLimit;

    public void Initialize(Quaternion orientation, Vector3 startingPos) {
        this.orientation = orientation;
        transform.position = startingPos;
    }

    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        Vector3 currentSpeed = rb.velocity;
        if (currentSpeed.magnitude > speedLimit) {
            rb.velocity = currentSpeed.normalized * speedLimit;
        } else {
            rb.AddForce(transform.forward * acceleration * Time.fixedDeltaTime);
        }
    }
}