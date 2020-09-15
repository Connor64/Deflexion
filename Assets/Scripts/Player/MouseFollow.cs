using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollow : MonoBehaviour {
    Vector3 position;
    void Update() {
        position = Input.mousePosition;

        position.x = ((position.x / Screen.width) - 0.5f) * 32;
        position.y = ((position.y / Screen.height) - 0.5f) * 18;
        // due to pixel perfect-ification, the mouse position gets all whack so I have to correct it

        transform.position = position;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "EnemyMouseTarget") {
            collision.gameObject.GetComponent<EnemyTargetAssist>().hovering = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "EnemyMouseTarget") {
            collision.gameObject.GetComponent<EnemyTargetAssist>().hovering = false;
        }
    }
}
