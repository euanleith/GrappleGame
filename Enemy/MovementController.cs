using System;
using UnityEngine;
using Random=UnityEngine.Random;

public class MovementController: MonoBehaviour
{
    // todo everything should stop for a second when you grapple an enemy for impact, and to prepare you to hit them when they fly towards you

    // todo make these variables of instantiations of movement - will want different e.g. speeds for idle and aggro, and e.g. moverange is specific to pingpong and similar
    
    public Vector2 moveRange = new Vector2(3, 0); // todo move this to pingpongmovement
    public Vector2 aggroRange = new Vector2(7, 3);
    public float decelerationSpeed = 1;
    public float stunDuration = 1f;
    public Vector2 startDir;
    public bool offset = true;
    
    [HideInInspector] public Vector2 collisionNormal;
    Vector2 currentDecelVelocity;
    [HideInInspector] public Vector2 direction;
    [HideInInspector] public Vector2 startPos;
    float stunCnt = 0f;
    bool stunned = false;
    Vector2 prevPosition;
    
    Movement currentMovement;
    Transform transform;
    Rigidbody2D rb;
    CombatController combatController;
    Enemy enemy;

    public void Init() {
        InitObjects();
        startPos = transform.position;  
        InitValues();
    }

    public void Reset() {
        InitObjects();
        InitValues();
    }

    public void InitObjects() {
        transform = gameObject.transform;
        rb = GetComponent<Rigidbody2D>();
        rb.useFullKinematicContacts = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        enemy = GetComponent<Enemy>();
    }

    public void InitValues() {
        transform.position = startPos;
        rb.bodyType = RigidbodyType2D.Kinematic;
        stunned = false;
        stunCnt = 0f;
        if (!offset) {
            transform.position = startPos;
            direction = startDir;
        }
        else {
            InitOffsets();
        }
        currentMovement = enemy.idleMovement;
    }

    void InitOffsets() {
        InitPositionOffset();
        InitDirectionOffset();
    }

    void InitPositionOffset() {
        Vector2 targetPos = new Vector2(
            Random.Range(startPos.x - moveRange.x, startPos.x + moveRange.x),
            Random.Range(startPos.y - moveRange.y, startPos.y - moveRange.y)
        );
        int ignoreRaycast = 1 << 8; // Enemy layer
        RaycastHit2D hit = Physics2D.Linecast (transform.position, targetPos, ~ignoreRaycast);
        if (!hit) {
            transform.position = targetPos;
        } else {
            Vector2 newPos = hit.point;
            if (hit.normal.x == 1) newPos.x += transform.localScale.x/2;
            if (hit.normal.x == -1) newPos.x -= transform.localScale.x/2;
            if (hit.normal.y == 1) newPos.x += transform.localScale.y/2;
            if (hit.normal.y == -1) newPos.x -= transform.localScale.y/2;
            transform.position = newPos;
        }
    }

    void InitDirectionOffset() {
        Vector2 newDir = new Vector2(Random.Range(0, 2),Random.Range(0, 2));
        // don't want starting direction to be 0
        if (newDir.x == 0) newDir.x = -1;
        if (newDir.y == 0) newDir.y = -1;
        direction = newDir;
    }

    public void FixedUpdate()
    {
        if (WithinPlayerRange() && enemy.aggroMovement != null) currentMovement = enemy.aggroMovement;
        else currentMovement = enemy.idleMovement;


        if (stunCnt >= 0) {
            stunCnt -= Time.deltaTime;
        } 
        else if (stunned) FinishStun();
        else if (currentDecelVelocity != Vector2.zero) {
            if (!isCollidingWithGrapple()) {
                currentDecelVelocity = SlowDown(currentDecelVelocity, decelerationSpeed);
            } else currentDecelVelocity = Vector2.zero;
        } 
        else if (rb.bodyType == RigidbodyType2D.Kinematic) {
            if (enemy.combatController.state == CombatController.State.idle || 
                enemy.combatController.state == CombatController.State.cooldown) {
                    if (enemy.combatController.state == CombatController.State.idle) {
                        Move();
                    }
                    float newScale = prevPosition.x < transform.position.x ? 1 : -1; // todo can i not use direction?
                    transform.localScale = new Vector2(newScale, transform.localScale.y);
            }
        }
        prevPosition = rb.position;
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == 2 || collision.gameObject.layer == 12) {
            Stun();
            rb.velocity = currentMovement.OnCollision(collision);
        } else {
            collisionNormal = collision.GetContact(0).normal;
            if (collisionNormal.x != 0) rb.velocity = new Vector2(0, rb.velocity.y);
            if (collisionNormal.y != 0) rb.velocity = new Vector2(rb.velocity.x, 0);
        }
    }

    public void OnTriggerEnter2D(Collider2D collider) {
        Stun();
        rb.velocity = currentMovement.OnHit(collider, transform);
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
        return Math.Abs(transform.position.x - enemy.player.transform.position.x) < aggroRange.x &&
            Math.Abs(transform.position.y - enemy.player.transform.position.y) < aggroRange.y;
    }

    public bool isColliding() {
        return collisionNormal != Vector2.zero;
    }

    public void Stun() {
        stunned = true;
        stunCnt = stunDuration;
        rb.velocity = Vector2.zero; // todo remove this since velocity is set in movement.oncollision/onhit?
    }

    public void FinishStun() {
        stunned = false;
        rb.velocity = Vector2.zero;
    }

    public void Move() {
        rb.MovePosition(currentMovement.Move(ref direction, transform, currentDecelVelocity, collisionNormal));
    }
}