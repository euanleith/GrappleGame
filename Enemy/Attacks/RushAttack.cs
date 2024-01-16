using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushAttack: Attack
{
    public float rushSpeed = 4;

    float direction;

    public void Start() {
        base.Start();
    }

    public override void SetHitbox(Transform enemy, Transform player) {
        base.SetHitbox(enemy, player);
        direction = enemy.position.x < player.position.x ? 1 : -1;
    }

    public override void KeepAttacking() {
        rb.MovePosition(rb.transform.position+new Vector3(rushSpeed*direction*Time.deltaTime,0,0));
    }

    public override bool IsFinished() {
        return movementController.collisionNormal != Vector2.zero;
    }
}
