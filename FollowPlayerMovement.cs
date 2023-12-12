using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerMovement : Movement
{
    private Transform player;

    public FollowPlayerMovement(Vector2 speed, Transform player) : base(speed) 
    {
        this.player = player;
    }

    public override Vector2 Move(Vector2 direction, Vector2 position, Vector2 currentDecelVelocity, Rigidbody2D rb) 
    {
        Vector2 newVel = speed;
        if (position.x > player.transform.position.x) newVel.x *= -1;
        if (position.y > player.transform.position.y) newVel.y *= -1;
        rb.MovePosition(new Vector2(position.x + newVel.x * Time.deltaTime, position.y + newVel.y * Time.deltaTime));
        return direction;
    }
}
