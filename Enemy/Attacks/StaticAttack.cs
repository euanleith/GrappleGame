using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticAttack : Attack
{
    public float aggroWindupDuration = 0.2f;

    public override void Start() {
        base.Start();
    }

    public override void Windup() {
        rb.MovePosition(rb.transform.position);
    }

    public override void KeepAttacking() {
        rb.MovePosition(rb.transform.position);
    }

    public override bool IsFinished() {
        float normalisedAnimTime = currentHitbox.GetComponent<Animator>()
            .GetCurrentAnimatorStateInfo(0)
            .normalizedTime; // todo would be nice to use cooldown = animationLength and iteratively subtract, but can't get animationLength :(
        return normalisedAnimTime > 1f;
    }
}
