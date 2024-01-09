using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticAttack : Attack
{
    public Rigidbody2D rb;
    public float aggroWindupDuration = 0.2f;

    public override void Windup() {
        rb.position = rb.position;
    }

    public override void KeepAttacking() {
        rb.position = rb.position;
    }

    public override bool IsFinished() {
        float normalisedAnimTime = currentHitbox.GetComponent<Animator>()
            .GetCurrentAnimatorStateInfo(0)
            .normalizedTime; // todo would be nice to use cooldown = animationLength and iteratively subtract, but can't get animationLength :(
        return normalisedAnimTime > 1f;
    }
}
