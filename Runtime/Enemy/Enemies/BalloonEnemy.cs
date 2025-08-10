using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonEnemy : Enemy
{
    public Rigidbody2D rb;
    public float riseSpeed = 1f;
    bool rising = false;

    public override void OnCollisionEnterWithGrapple() {
        // base.OnCollisionEnterWithGrapple(); // deliberately not running this since don't want to be able to move enemy with grapple
        rising = true;
    }

    public override void Update() {
        base.Update();
        if (rising) {
            rb.transform.position = rb.transform.position + new Vector3(0, riseSpeed * Time.deltaTime, 0); // note that this doesn't process collisions, so it will continue rising above the ceiling
            // todo maybe if above the ceiling, should delete this
        }
    }
}
