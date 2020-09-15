using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using UnityEngine;

public class SpeedLine : MonoBehaviour {
    private float speed;
    private Color color;

    private float xPosition;
    private float zPosition;
    private float yScale;

    public void Initialize(float alpha) {
        color.a = alpha;
        GetComponent<SpriteRenderer>().color = color;
    }

    void Awake() {
        color = GetComponent<SpriteRenderer>().color;

        xPosition = UnityEngine.Random.Range(-16.0f, 16.0f);
        zPosition = UnityEngine.Random.Range(-5.0f, 5.0f);
        yScale = UnityEngine.Random.Range(0.5f, 1.5f);

        transform.position = new Vector3(xPosition, 10.0f, zPosition);
        transform.localScale = new Vector3(1, 0, 1);

        speed = UnityEngine.Random.Range(25.0f, 30.0f);
    }

    void Update() {
        transform.position = new Vector3(transform.position.x, transform.position.y - (speed * Time.deltaTime), transform.position.z);
        if (transform.position.y < -13.0f || GetComponent<SpriteRenderer>().color.a <= 0) {
            Destroy(gameObject);
        }

        if (CameraEffects.instance.areSpeedLines()) {
            if (transform.localScale.y < yScale) {
                transform.localScale = new Vector3(1, transform.localScale.y + 2.0f * Time.deltaTime, 1);
            }
            if (transform.localScale.y > yScale) {
                transform.localScale = new Vector3(1, yScale, 1);
            }
        } else {
            if (transform.localScale.y > 0) {
                transform.localScale = new Vector3(1, transform.localScale.y - 1.5f * Time.deltaTime, 1);
            }
            if (transform.localScale.y < 0) {
                transform.localScale = new Vector3(1, 0, 1);
            }
        }
        if (!CameraEffects.instance.areSpeedLines()) {
            color.a = color.a - 2.5f * Time.unscaledDeltaTime;
            GetComponent<SpriteRenderer>().color = color;
        }
    }
}