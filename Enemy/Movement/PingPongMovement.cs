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
    public override Vector2 Move(ref Vector2 direction, Transform transform, Vector2 currentDecelVelocity, Vector2 collisionNormal) 
    {
        if (moveRange.x == 0) {
            direction.x = 0;
        }
        else if (transform.position.x < startPos.x - moveRange.x || collisionNormal.x > 0) {
            direction.x = 1;
        }
        else if (transform.position.x > startPos.x + moveRange.x || collisionNormal.x < 0) {
            direction.x = -1;
        }

        if (moveRange.y == 0) {
            direction.y = 0;
        }
        else if (transform.position.y < startPos.y - moveRange.y || collisionNormal.y > 0) {
            direction.y = 1;
        }
        else if (transform.position.y > startPos.y + moveRange.y || collisionNormal.y < 0) {
            direction.y = -1;
        }
        return currentDecelVelocity.normalized * Time.deltaTime + new Vector2(transform.position.x + (speed.x * direction.x * Time.deltaTime), transform.position.y + (speed.y * direction.y * Time.deltaTime));
    }

    public override Vector2 OnCollision(Collision2D collision) {
        Vector2 normal = collision.GetContact(0).normal;
        if (normal.y == 0) normal.y = 1; 
        if (normal.x == 0) normal.x = 1;
        return normal * speed;
    }

    public override Vector2 OnHit(Collider2D collider, Transform transform) {
        Vector2 relativePosition = Vector2.zero;
        relativePosition.x = collider.transform.position.x > transform.position.x ? -1 : 1;
        relativePosition.y = collider.transform.position.y > transform.position.y ? -1 : 1;
        return relativePosition * speed;
    }
}