using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingRope : MonoBehaviour
{
    public bool breakable = false; // todo i shouldn't have 2 variables for this this is awful just move it all into one script in Swing

    public void OnTriggerEnter2D(Collider2D collider) {
        if (breakable && collider.gameObject.layer == 2) { // todo PLAYER_ATTACK_LAYER) {
            transform.parent.GetComponentInChildren<SpringJoint2D>().enabled = false;
            Debug.Log(transform.parent.GetComponentInChildren<SpringJoint2D>().gameObject.name);
        }
    }
}
