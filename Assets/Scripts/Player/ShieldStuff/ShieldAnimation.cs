using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldAnimation : MonoBehaviour {

    public Animator animator;
    public Shield playerShield;

    void Start() {
        
    }

    void Update() {
        if (!PauseMenu.gamePaused) {
            animationCheck();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Laser") {
            animator.SetBool("reflected", true);
        }
    }

    public void animationCheck() {
        //if (animator.GetCurrentAnimatorStateInfo(0).IsName("Shield_idle") && animator.GetBool("reflected")) {
        //    animator.SetBool("reflected", false);
        //}

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Shield_recoil")) {
            animator.SetBool("reflected", false);
        }
    }
}
