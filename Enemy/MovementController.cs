using System;
using UnityEngine;

public class MovementController: MonoBehaviour
{
    // todo everything should stop for a second when you grapple an enemy for impact, and to prepare you to hit them when they fly towards you
    // todo problems when grappling enemies;
    //  when pulling them towards you, if you hit them and they dont die, you'll get hurt. so need to have bounce back on hit

    // todo make these variables of instantiations of movement - will want different e.g. speeds for idle and aggro, and e.g. moverange is specific to pingpong and similar
    
    public Vector2 moveRange = new Vector2(3, 0); // todo move this to pingpongmovement
    public Vector2 aggroRange = new Vector2(7, 3);
    public float decelerationSpeed = 1;
    
    [HideInInspector] public Vector2 collisionNormal;
    Vector2 currentDecelVelocity;
    Vector2 direction;
    [HideInInspector] public Vector2 startPos;
    float stunCnt = 0f;
    public float stunDuration = 1f;
    
    Movement idleMovement;
    Movement aggroMovement;
    Movement currentMovement;
    Transform transform;
    Transform player;
    Rigidbody2D rb;

    public void Start() {
        transform = gameObject.transform;        
        startPos = transform.position;
        player = GetComponent<Enemy>().player;
        direction = new Vector2(1, 1);
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.useFullKinematicContacts = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        idleMovement = GetComponent<Enemy>().idleMovement;
        aggroMovement = GetComponent<Enemy>().aggroMovement;
        currentMovement = idleMovement;
    }

    public void FixedUpdate()
    {
        if (WithinPlayerRange()) currentMovement = aggroMovement;
        else currentMovement = idleMovement;

        if (stunCnt > 0) {
            stunCnt -= Time.deltaTime;
        }
        else if (currentDecelVelocity != Vector2.zero) {
            if (!isCollidingWithGrapple()) {
                currentDecelVelocity = SlowDown(currentDecelVelocity, decelerationSpeed);
            } else currentDecelVelocity = Vector2.zero;
        } 
        else if (rb.bodyType == RigidbodyType2D.Kinematic) {
            direction = currentMovement.Move(direction, transform.position, currentDecelVelocity, rb, collisionNormal);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == 2 || collision.gameObject.layer == 12) {
            Stun();
            currentMovement.OnCollision(collision, rb);
        } else {
            collisionNormal = collision.GetContact(0).normal;
            if (collisionNormal.x != 0) rb.velocity = new Vector2(0, rb.velocity.y);
            if (collisionNormal.y != 0) rb.velocity = new Vector2(rb.velocity.x, 0);
        }
    }

    public void OnTriggerEnter2D(Collider2D collider) {
        Stun();
        currentMovement.OnHit(collider, rb);
    }

    public void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.layer == 2 || collision.gameObject.layer == 12) return;
        collisionNormal = collision.GetContact(0).normal;
        //if (collisionNormal.x > 0) rb.MovePosition(new Vector2(collision.transform.position.x - collision.transform.localScale.x/2, rb.position.y));
        //if (collisionNormal.x < 0) rb.MovePosition(new Vector2(collision.transform.position.x + collision.transform.localScale.x/2, rb.position.y));
        //if (collisionNormal.y > 0) rb.MovePosition(new Vector2(rb.position.x, collision.transform.position.y - collision.transform.localScale.y/2));
       // if (collisionNormal.y < 0) rb.MovePosition(new Vector2(rb.position.x, collision.transform.position.y + collision.transform.localScale.y/2));
    }

    public void OnCollisionExit2D() {
        collisionNormal = Vector2.zero;
    }

    // todo would it be better to normally be a dynamic rb, then change to kinematic when colliding with player?
    // todo maybe this should only happen with transform grapple, or at least act differently for each type of grapple
    public void OnCollisionEnterWithGrapple() {
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public void OnCollisionExitWithGrapple() {
        currentDecelVelocity = rb.velocity;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public bool isCollidingWithGrapple() {
        return rb.bodyType == RigidbodyType2D.Dynamic;
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

    public bool WithinPlayerRange() {
        return Math.Abs(transform.position.x - player.transform.position.x) < aggroRange.x &&
            Math.Abs(transform.position.y - player.transform.position.y) < aggroRange.y;
    }

    public bool isColliding() {
        return collisionNormal != Vector2.zero;
    }

    public void Stun() {
        stunCnt = stunDuration;
        rb.velocity = Vector2.zero;
    }
}
