using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricEnemy : Enemy
{
    public float stunDuration = 1f;
    float cnt = 0;

    public override void OnCollisionEnterWithGrapple() {
        // base.OnCollisionEnterWithGrapple(); // deliberately not running this since don't want to be able to move enemy with grapple
        cnt = stunDuration;
        player.gameObject.GetComponent<PlayerControls>().StartStun();
    }

    public override void Update() {
        base.Update();
        if (cnt > 0) {
            cnt -= Time.deltaTime;
        } else {
            player.gameObject.GetComponent<PlayerControls>().FinishStun();
        }
    }
}
