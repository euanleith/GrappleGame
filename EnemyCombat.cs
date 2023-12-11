using System;
using UnityEngine;

public abstract class EnemyCombat : MonoBehaviour
{
    // todo maybe rename to just enemy? or separate in combat and movement
    // todo dont need to run when not in the room. also make sure they dont leave the room
    // todo everything should stop for a second when you grapple an enemy for impact, and to prepare you to hit them when they fly towards you
    // todo problems when grappling enemies;
    //  they go of their set path & can clip into the floor - the problem is that their velocity isn't zero? so what do i do about that? set to 0 after a certain time? add a deceleration?
    //  when pulling them towards you, if you hit them and they dont die, you'll get hurt. so need to have bounce back on hit6

    public int damage = 1;
    public int health = 1;
    public float countdown;
    public float windupDuration; // todo this should be per attack
    public float cooldownDuration; // todo this should be per attack
    public Transform hitbox;
    public Transform leftHitbox;
    public Transform rightHitbox;
    public enum State
    {
        windup,
        attacking,
        cooldown
    }
    public State state;
    public Transform player;
    public Vector2 aggroRange = new Vector2(7, 3); // todo maybe rename aggroRange and attackRange
    public Vector2 minAttackRange = new Vector2(3, 2);
    private new Transform transform;
    [HideInInspector] public Vector2 startPos;
    public Vector2 speed = new Vector2(2, 1);
    public Rigidbody2D rb;
    public float decelerationSpeed = 1;
    public Vector2 currentDecelVelocity;
    public Vector2 collisionNormal;
    public Vector2 moveRange = new Vector2(3, 0);
    
    [HideInInspector] public Vector2 direction = new Vector2(1, 1);

    // Start is called before the first frame update
    void Start()
    {
        state = State.cooldown;
        transform = GetComponent<Transform>();
        startPos = transform.position;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.useFullKinematicContacts = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentDecelVelocity != Vector2.zero) {
            if (!ProcessCollisions(currentDecelVelocity, collisionNormal)) {
                currentDecelVelocity = SlowDown(currentDecelVelocity, decelerationSpeed);
            } else currentDecelVelocity = Vector2.zero;
        } 
        else if (rb.bodyType == RigidbodyType2D.Kinematic) {
            if (!ProcessCollisions(currentDecelVelocity, collisionNormal)) {
                if (WithinPlayerRange()) {
                    MoveAggro(currentDecelVelocity, collisionNormal);
                } else {
                    MoveIdle(currentDecelVelocity);
                }
            }
        }
        switch (state)
        {
            case State.cooldown:
                if (countdown <= 0 && 
                    Math.Abs(transform.position.x - player.transform.position.x) < hitbox.localScale.x + minAttackRange.x &&
                    Math.Abs(transform.position.y - player.transform.position.y) < minAttackRange.y)
                {    
                    countdown = windupDuration;
                    GetComponent<SpriteRenderer>().color = Color.red;
                    state = State.windup;
                    hitbox = transform.position.x > player.transform.position.x ? leftHitbox : rightHitbox;
                    
                }
                break;
            case State.attacking:
                float normalisedAnimTime = hitbox.GetComponent<Animator>()
                    .GetCurrentAnimatorStateInfo(0)
                    .normalizedTime; // todo would be nice to use cooldown = animationLength and iteratively subtract, but can't get animationLength :(
                if (normalisedAnimTime > 1f) // if finished attacking
                {
                    countdown = cooldownDuration;
                    hitbox.GetComponent<SpriteRenderer>().enabled = false;
                    hitbox.GetComponent<BoxCollider2D>().enabled = false;
                    hitbox.GetComponent<Animator>().StopPlayback();
                    state = State.cooldown;
                }
                break;
            case State.windup:
                if (countdown <= 0)
                {
                    hitbox.GetComponent<SpriteRenderer>().enabled = true;
                    hitbox.GetComponent<BoxCollider2D>().enabled = true;
                    hitbox.GetComponent<Animator>().Play(0);
                    GetComponent<SpriteRenderer>().color = Color.white;
                    state = State.attacking;
                }
                break;
        }   
        if (countdown > 0) {
            countdown -= Time.deltaTime;
        }
    }

    public void GetHit(int damage) {
        health -= damage;
        //GetComponent<SpriteRenderer>().color = Color.red;
        // todo get hit animation, and maybe bounce backwards too, and stun both them and player (maybe stun instead of hurt player, or maybe it depends on the enemy?)
        if (health <= 0) {
            Debug.Log("BLEGH!!!!");
            gameObject.SetActive(false);
        } else {
            Debug.Log("ow :(");
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

    // todo name
    // todo this could be the standard idle, which can be overwritten if i want something else
    // todo surely it would be nice to make animations instead of writing their movement programmaticaly
    //  maybe i could have an animator which is enabled when idle, and disabled when Aggro
    //  but then i can't return to the original idle range
    public void MoveIdle(Vector2 currentDecelVelocity) {
        Vector2 temp = direction;
        if (transform.position.x < startPos.x - moveRange.x) {
            direction.x = 1;
        }
        else if (transform.position.x > startPos.x + moveRange.x) {
            direction.x = -1;
        }
        if (transform.position.y < startPos.y - moveRange.y) {
            direction.y = 1;
        }
        else if (transform.position.y > startPos.y + moveRange.y) {
            direction.y = -1;
        }
            
        rb.MovePosition(currentDecelVelocity.normalized * Time.deltaTime + new Vector2(transform.position.x + (speed.x * direction.x * Time.deltaTime), transform.position.y + (speed.y * direction.y * Time.deltaTime)));
    }

    public bool WithinPlayerRange() {
        return Math.Abs(transform.position.x - player.transform.position.x) < aggroRange.x &&
            Math.Abs(transform.position.y - player.transform.position.y) < aggroRange.y;
    }

    // todo name - its only when within range of player
    public abstract void MoveAggro(Vector2 currentDecelVelocity, Vector2 collisionNormal); // todo these parameters are confusing, since currentDecelVelocity refers to collision with player, but collisionNormal refers to other collisions
   // todo what i really want is a bunch of general functions for idlemovement, aggromovement, attack, and onhit functions, then i can create class MyEnemyName and take this idlemovement function and that onhit function etc
   //   so would i create a parent class for, say, idlemovement, then subclasses for each type. then have the general Enemy class have a global variable IdleMovement, which is instantiated with a specific subclass by the subclass of Enemy?
}
