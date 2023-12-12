using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongMovement : Movement
{
    private Vector2 startPos;
    private Vector2 moveRange;

    public PingPongMovement(Vector2 speed, Vector2 startPos, Vector2 moveRange) : base(speed) {
        this.startPos = startPos;
        this.moveRange = moveRange;
    }

    public override Vector2 Move(Vector2 direction, Vector2 position, Vector2 currentDecelVelocity, Rigidbody2D rb) 
    {
        if (position.x < startPos.x - moveRange.x) {
            direction.x = 1;
        }
        else if (position.x > startPos.x + moveRange.x) {
            direction.x = -1;
        }
        if (position.y < startPos.y - moveRange.y) {
            direction.y = 1;
        }
        else if (position.y > startPos.y + moveRange.y) {
            direction.y = -1;
        }
        rb.MovePosition(currentDecelVelocity.normalized * Time.deltaTime + new Vector2(position.x + (speed.x * direction.x * Time.deltaTime), position.y + (speed.y * direction.y * Time.deltaTime)));
        return direction; // todo i dont really want to return this. i need to be able to alter direction without returning it, normally this would be done by using the same pointer as the parameter, but it seems like c sharp avoids this by default?
        // https://www.reddit.com/r/csharp/comments/jr0qs4/why_do_we_create_a_new_variable_to_make_a_variable/ - maybe need to add 'new' somewhere, or initialise with new before running this
    }
}
