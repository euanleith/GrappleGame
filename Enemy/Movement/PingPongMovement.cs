using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongMovement : Movement
{
    private Vector2 startPos;
    private Vector2 moveRange;
    // todo add initial direction (currently always up and right)

    public void Start() {
        startPos = gameObject.GetComponentInParent<MovementController>().transform.position;
        moveRange = gameObject.GetComponentInParent<MovementController>().moveRange;
    }

    // todo wiggles when colliding with object on way back to initial range
    public override Vector2 Move(Vector2 direction, Vector2 position, Vector2 currentDecelVelocity, Rigidbody2D rb, Vector2 collisionNormal) 
    {
        if (moveRange.x == 0) {
            direction.x = 0;
        }
        else if (position.x < startPos.x - moveRange.x || collisionNormal.x > 0) {
            direction.x = 1;
        }
        else if (position.x > startPos.x + moveRange.x || collisionNormal.x < 0) {
            direction.x = -1;
        }

        if (moveRange.y == 0) {
            direction.y = 0;
        }
        else if (position.y < startPos.y - moveRange.y || collisionNormal.y > 0) {
            direction.y = 1;
        }
        else if (position.y > startPos.y + moveRange.y || collisionNormal.y < 0) {
            direction.y = -1;
        }
        rb.MovePosition(currentDecelVelocity.normalized * Time.deltaTime + new Vector2(position.x + (speed.x * direction.x * Time.deltaTime), position.y + (speed.y * direction.y * Time.deltaTime)));
        return direction; // todo i dont really want to return this. i need to be able to alter direction without returning it, normally this would be done by using the same pointer as the parameter, but it seems like c sharp avoids this by default?
        // https://www.reddit.com/r/csharp/comments/jr0qs4/why_do_we_create_a_new_variable_to_make_a_variable/ - maybe need to add 'new' somewhere, or initialise with new before running this
        // or i can use 'ref direction'
        //  and with this gone, can return the movement vector, helping with isolation and meaning don't have to have rb as a parameter
    }

    // todo make everything here return velocities for rb instead of taking it in as parameter
    public override void OnCollision(Collision2D collision, Rigidbody2D rb) {
        Vector2 normal = collision.GetContact(0).normal;
        if (normal.y == 0) normal.y = 1; 
        if (normal.x == 0) normal.x = 1;
        rb.velocity = normal * speed;
    }

    public override void OnHit(Collider2D collider, Rigidbody2D rb) {
        Vector2 relativePosition = Vector2.zero;
        relativePosition.x = collider.transform.position.x > rb.transform.position.x ? -1 : 1;
        relativePosition.y = collider.transform.position.y > rb.transform.position.y ? -1 : 1;
        rb.velocity = relativePosition * speed;
    }
}
