using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetAssist : MonoBehaviour {

    public GameObject shieldContainer;
    private GameObject target;
    private bool lockedOn = false;
    [HideInInspector]
    public bool hovering = false;

    private Shield shield;
    private Shield parentShield;

    private void Start() {
        target = transform.GetChild(0).gameObject;

        shield = shieldContainer.GetComponent<Shield>();
        parentShield = shieldContainer.GetComponentInParent<Shield>();
    }

    private void Update() {
        clickConfirm();
        if (lockedOn) {
            if (!target.activeSelf) {
                target.SetActive(true);
            }
            shield.setLockedOn(true);
            parentShield.lookAt(transform.position);
        } else {
            if (shield.isLockedOn()) {
                shield.setLockedOn(false);
                lockedOn = false;
                hovering = false;
                target.SetActive(false);
            }
        }
    }

    void clickConfirm() {
        if (hovering && Input.GetMouseButtonDown(0)) {
            lockedOn = true;
        } else if (!hovering && Input.GetMouseButtonDown(0)) {
            lockedOn = false;
        }

        //lockedOn = hovering && Input.GetMouseButtonDown(0) ? true : !hovering && Input.GetMouseButtonDown(0) ? false;
    }

    //private void OnTriggerEnter2D(Collider2D collision) {
    //    if (collision.gameObject.tag == "Mouse") {
    //        hovering = true;
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision) {
    //    if (collision.gameObject.tag == "Mouse") {
    //        hovering = false;
    //    }
    //}
}
