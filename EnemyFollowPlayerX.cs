using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollowPlayerX : EnemyCombat
{
    public override void MoveAggro(Vector2 currentDecelVelocity, Vector2 collisionNormal) {
        Vector2 newVel = speed;
        if (transform.position.x > player.transform.position.x) newVel.x *= -1;
        rb.MovePosition(new Vector3(transform.position.x + newVel.x * Time.deltaTime, transform.position.y, transform.position.z));
    }
}
