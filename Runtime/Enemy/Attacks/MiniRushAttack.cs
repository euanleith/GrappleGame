using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// todo name - rush enemy which stops after some time
public class MiniRushAttack: Attack
{
    public float rushSpeed = 4;
    public float rushDuration = 1f;
    float rushTime;

    float direction;

    public override void Start() {
        base.Start();
    }

    public override void SetHitbox(Transform enemy, Transform player) {
        base.SetHitbox(enemy, player);
        direction = enemy.position.x < player.position.x ? 1 : -1;
    }

    public override void StartAttacking() {
        base.StartAttacking();
        rushTime = rushDuration;
    }

    public override void KeepAttacking() {
        rb.MovePosition(rb.transform.position+new Vector3(rushSpeed*direction*Time.deltaTime,0,0));
        rushTime -= Time.deltaTime;
    }

    public override bool IsFinished() {
        return rushTime < 0;
    }
}
