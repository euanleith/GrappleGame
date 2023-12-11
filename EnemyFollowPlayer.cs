using System;
using UnityEngine;

// todo rename enemyfollowplayer or something
public class EnemyFollowPlayer : EnemyCombat
{
    public override void MoveAggro(Vector2 currentDecelVelocity, Vector2 collisionNormal) {
        Vector2 newVel = speed;
        if (transform.position.x > player.transform.position.x) newVel.x *= -1;
        if (transform.position.y > player.transform.position.y) newVel.y *= -1;
        rb.MovePosition(new Vector3(transform.position.x + newVel.x * Time.deltaTime, transform.position.y + newVel.y * Time.deltaTime, transform.position.z));
    }
}
