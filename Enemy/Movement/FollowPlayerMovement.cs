using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerMovement : Movement
{
    Transform player;
    public Vector2 maxDistanceFromPlayer;

    public void Start() {
        player = GetComponentInParent<Enemy>().player;
    }

    public override Vector2 Move(Vector2 direction, Vector2 position, Vector2 currentDecelVelocity, Rigidbody2D rb) 
    {
        Vector2 newVel = Vector2.zero;
        if (position.x < player.transform.position.x - maxDistanceFromPlayer.x) newVel.x = speed.x;
        else if (position.x > player.transform.position.x + maxDistanceFromPlayer.x) newVel.x = -speed.x;
        if (position.y < player.transform.position.y - maxDistanceFromPlayer.y) newVel.y = speed.y;
        else if (position.y > player.transform.position.y + maxDistanceFromPlayer.y) newVel.y = -speed.y;
        rb.MovePosition(new Vector2(position.x + newVel.x * Time.deltaTime, position.y + newVel.y * Time.deltaTime)); // todo this is probably being overwritten
        //rb.position = new Vector2(position.x + newVel.x * Time.deltaTime, position.y + newVel.y * Time.deltaTime);
        return direction;
    }
}
