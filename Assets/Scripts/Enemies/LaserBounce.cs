using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBounce : MonoBehaviour {
    private Vector2 initialVelocity;
    private Vector2 currentVelocity;
    private Vector3 direction;
    private float vel = 20.0f;
    private Rigidbody2D rb;
    private float speed;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        initialVelocity = rb.velocity;
    }

    void Update() {
        currentVelocity = rb.velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.name == "Shield") {
            speed = currentVelocity.magnitude;
            direction = Vector3.Reflect(currentVelocity.normalized, collision.contacts[0].normal);
            rb.velocity = direction * Mathf.Max(speed, vel);
        }
    }
}