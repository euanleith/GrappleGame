using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricEnemy : Enemy
{
    public float stunDuration = 1f;

    public override void OnCollisionEnterWithGrapple() {
        // base.OnCollisionEnterWithGrapple(); // deliberately not running this since don't want to be able to move enemy with grapple
        player.gameObject.GetComponent<PlayerControls>().Stun(stunDuration);
    }

    public override void Update() {
        base.Update();
    }
}
