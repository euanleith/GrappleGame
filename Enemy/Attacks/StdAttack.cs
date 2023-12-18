using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StdAttack: Attack
{
    public override void KeepAttacking() {
    }

    public override bool IsFinished() {
        float normalisedAnimTime = currentHitbox.GetComponent<Animator>()
            .GetCurrentAnimatorStateInfo(0)
            .normalizedTime; // todo would be nice to use cooldown = animationLength and iteratively subtract, but can't get animationLength :(
        return normalisedAnimTime > 1f;
    }
}
