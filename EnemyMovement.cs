using System;
using UnityEngine;

public class EnemyMovement: MonoBehaviour
{
    // todo everything should stop for a second when you grapple an enemy for impact, and to prepare you to hit them when they fly towards you
    // todo problems when grappling enemies;
    //  they go of their set path & can clip into the floor - the problem is that their velocity isn't zero? so what do i do about that? set to 0 after a certain time? add a deceleration?
    //  when pulling them towards you, if you hit them and they dont die, you'll get hurt. so need to have bounce back on hit

    public Vector2 collisionNormal;
    public Vector2 currentDecelVelocity;
    public Vector2 direction;
    public Vector2 aggroRange = new Vector2(7, 3);
    public float decelerationSpeed = 1;
    public Movement idleMovement;
    public Movement aggroMovement;
    public Vector2 speed = new Vector2(2, 1);
    public Vector2 moveRange = new Vector2(3, 0);

    Transform transform;
    Transform player;
    Rigidbody2D rb;

    public void Start() {
        transform = gameObject.transform;        
        player = GetComponent<Enemy>().player;
        rb = GetComponent<Rigidbody2D>();
        direction = new Vector2(1,1);
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.useFullKinematicContacts = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    public void FixedUpdate()
    {
        if (currentDecelVelocity != Vector2.zero) {
            if (!ProcessCollisions(currentDecelVelocity, collisionNormal)) { // todo currently grappling an enemy stops their idle movement, do i want this?
                currentDecelVelocity = SlowDown(currentDecelVelocity, decelerationSpeed);
            } else currentDecelVelocity = Vector2.zero;
        } 
        else if (rb.bodyType == RigidbodyType2D.Kinematic) {
            if (!ProcessCollisions(currentDecelVelocity, collisionNormal)) { // todo wrong, right now if it collides with the ground itll stop following player
                if (WithinPlayerRange()) {
                    aggroMovement.Move(direction, transform.position, currentDecelVelocity, rb);
                } else {
                    direction = idleMovement.Move(direction, transform.position, currentDecelVelocity, rb);
                }
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        // todo i want to negate the collision for enemies with a weak spot if player also hits the non-weak spot
        collisionNormal = collision.GetContact(0).normal;
    }

    public void OnCollisionExit2D() {
        collisionNormal = Vector2.zero;
    }

    public void OnCollisionEnterWithGrapple() {
        rb.bodyType = RigidbodyType2D.Dynamic;
        // todo can i stop the jiggling?
    }

    public void OnCollisionExitWithGrapple() {
        currentDecelVelocity = rb.velocity;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }
    
    // todo this is the worst code ive ever written
    private Vector2 SlowDown(Vector2 currentDecelVelocity, float decelerationSpeed) {
        Vector2 decel = Vector2.zero;
        if (currentDecelVelocity.x > 0) { 
            if (currentDecelVelocity.x < 1) decel.x = -currentDecelVelocity.x;
            else decel.x = - decelerationSpeed * Time.deltaTime;
        }
        if (currentDecelVelocity.x < 0) {
            if (currentDecelVelocity.x > -1) decel.x = -currentDecelVelocity.x;
            else decel.x = decelerationSpeed * Time.deltaTime;
        }
        if (currentDecelVelocity.y > 0) {
            if (currentDecelVelocity.y < 1) decel.y = -currentDecelVelocity.y;
            else decel.y = - decelerationSpeed * Time.deltaTime;
        }
        if (currentDecelVelocity.y < 0) {
            if (currentDecelVelocity.y > -1) decel.y = -currentDecelVelocity.y;
            else decel.y = decelerationSpeed * Time.deltaTime;
        }
        if (collisionNormal.x != 0) decel.x = -currentDecelVelocity.x;
        if (collisionNormal.y != 0) decel.y = -currentDecelVelocity.y;
        return currentDecelVelocity + decel;
    }

    // todo name
    //  todo would it be better to normally be a dynamic rb, then change to kinematic when colliding with player?
    public bool ProcessCollisions(Vector2 currentDecelVelocity, Vector2 collisionNormal) {
        bool colliding = false;
        if (collisionNormal.x == 1) {
            direction.x = 1;
            colliding = true;
        }
        if (collisionNormal.x == -1) {
            direction.x = -1;
            colliding = true;
        }
        // todo top one isn't working??
        if (collisionNormal.y == 1) {
            direction.y = 1;
            colliding = true;
        }
        if (collisionNormal.y == -1) {
            direction.y = -1;
            colliding = true;
        }
            
        if (colliding) {
            rb.MovePosition(currentDecelVelocity.normalized * Time.deltaTime + new Vector2(transform.position.x + (speed.x * direction.x * Time.deltaTime), transform.position.y + (speed.y * direction.y * Time.deltaTime)));
        }
        return colliding;
    }

    public bool WithinPlayerRange() {
        return Math.Abs(transform.position.x - player.transform.position.x) < aggroRange.x &&
            Math.Abs(transform.position.y - player.transform.position.y) < aggroRange.y;
    }
}
