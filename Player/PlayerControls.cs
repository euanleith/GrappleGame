using System;
using System.Collections;
using System.ComponentModel.Design;
using UnityEngine;

// todo rename PlayerMovement, or just Movement
public class PlayerControls : MonoBehaviour
{
    // todo order these

    public Rigidbody2D rb;
    public new Camera camera;
    public GrapplingGun grapple;
    public Menu menu;

    public float groundSpeed = 7f;
    public float jumpSpeed = 7f;
    public float forwardAirSpeed = 5f;
    public float backwardAirSpeed = 20f;
    public float fallSpeed = 0.01f;
    public float fastFallSpeed = 0.2f;
    public float minFastFallThreshold = -0.4f; // todo maybe have a min time holding down as well / instead?
    public float wallJumpSpeed = 2f;
    public float wallClimbSpeed = 7f;
    public float wallFallSpeed = 0.001f;
    public float wallFastFallSpeed = 0.01f;
    public float grappleMoveSpeed = 5f;
    public float grappleMoveSpeedY = 100f;
    public float stdBounceSpeed;
    public Vector2 knockbackSpeed = new Vector2(1, 2);

    public float maxSpeed = 20f;


    public float moveX;
    public float moveY;

    public float hitWallNormal; // todo not great name - it's reffering to the normal of the wall currently being collided with, if there is one, and 0 otherwise
    public float minWallAngle = 0.5f;

    private Collider2D playerCollider;

    private Vector2 previousPos;

    public Vector2 velocityOfGround;

    public bool isGrounded = false;

    public float minSlope = 0.5f; // todo might need to change this for slopes

    Collider2D lastGroundCollision; // need this since OnCollisionExit2D doesn't contain collision info of the exited collision
    int nCurrentCollisions = 0;

    public bool stunned = false;
    public const float stunDuration = 1f;
    float stunCnt = 0f;

    public const float jumpCooldownDuration = 0.1f;
    float jumpCooldown = 0f;

    bool delayedSwingCollision = false;


    void Start()
    {
        previousPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (stunCnt > 0) {
            stunCnt -= Time.deltaTime;
            return;
        } else if (stunned) FinishStun();

        if (jumpCooldown > 0) {
            jumpCooldown -= Time.deltaTime;
        }

        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");

        if (!(Input.GetButton("Fire1") || Input.GetButton("Fire2"))) // todo if not grappling
        {
            // todo would making this a switch make it clearer? or maybe just move the booleans to functions, and maybe also the contents of each if, though idk about the latter
            if ((isGrounded || (hitWallNormal != 0)) && Input.GetButtonDown("Jump") && !menu.enabled) { // jumping
                if (isGrounded && lastGroundCollision.gameObject.layer == 22 && moveY < 0) { // TraversablePlatform layer
                    // todo or holding down while land on TraversablePlatform?
                    lastGroundCollision.gameObject.GetComponent<TraversablePlatform>().Traverse();
                    isGrounded = false;
                }
                else Jump();
            }
            else if (isGrounded && 
                    hitWallNormal != 0 && 
                    rb.velocity.x * hitWallNormal < 0)
            {
                rb.velocity = new Vector2(0, wallClimbSpeed);
            }
            else if (isGrounded && jumpCooldown <= 0) { // moving along ground
                rb.velocity = new Vector2(moveX * groundSpeed, 0f);
            } else { // moving in air
                float airSpeedX = moveX * rb.velocity.x > 0 ? forwardAirSpeed : backwardAirSpeed;
                float airSpeedY;
                if (hitWallNormal != 0 && moveX * hitWallNormal < 0) {
                    airSpeedY = moveY < minFastFallThreshold ? wallFastFallSpeed : wallFallSpeed; 
                } else {
                    airSpeedY = moveY < minFastFallThreshold ? fastFallSpeed : fallSpeed;
                }
                rb.velocity = new Vector2((rb.velocity.x + (moveX * airSpeedX * Time.deltaTime)), rb.velocity.y - (airSpeedY * Time.deltaTime)); // todo is this accelerating fall? i dont want it to - * deltaTime?
            }
        } else {
            // todo make only possible for transform grapple after reaching original destination? otherwise its weird when e.g. you transform grapple straight down while holding down
            rb.velocity = new Vector2((rb.velocity.x + (Time.deltaTime * moveX * grappleMoveSpeed)), rb.velocity.y);
        }

        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
        // todo clamp angular velocity too?

        menu.ManualUpdate();
    }

    private void LateUpdate() {
        if (ShouldUnlinkFromSwing()) {
            UnlinkFromSwing();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        nCurrentCollisions++;
        hitWallNormal = 0f;
        ContactPoint2D contact = collision.GetContact(0); // only need 1 contact point as player collider is box
        if (Math.Abs(contact.normal.x) > minWallAngle)
        {
            hitWallNormal = contact.normal.x;
        } else if (contact.normal.y > minSlope) {
            lastGroundCollision = contact.collider;
            isGrounded = true;
            grapple.OnCollisionWithGround();
        }
        if (!grapple.isEnabled() &&
            IsAbove(gameObject, collision.gameObject))
        {
            if (collision.gameObject.GetComponent<SwingPlatform>() != null) {
                LinkToSwing(collision.transform);
            } else {
                gameObject.transform.parent = collision.gameObject.transform;
            }
        }
    }

    public Vector2 GetVelocityOfGround(GameObject ground) {
        try {
            return ground.GetComponent<Rigidbody2D>().velocity;
            // todo im pretty sure animations change the velocity, so this should work for both
            //Animator anim = ground.gameObject.GetComponent<Animator>();
            //return anim.velocity;
            // todo can i also convert rotation velocity to velocity? https://gamedev.stackexchange.com/questions/167428/how-to-convert-rotatearound-speed-to-directional-speed-in-unity?
        } catch {
            return Vector2.zero;
        }
    }

    void OnCollisionExit2D(Collision2D collision) 
    {
        nCurrentCollisions--;
        hitWallNormal = 0f;

        // todo there's other cases;
        //  on hit by enemy..?
        //  on land on swing then die?
        // if player is within horizontal bounds of platform and hasnt jumped/grappled

        //Debug.Log("Collision exit");
        //Debug.Log("grapple enabled: " + grapple.isEnabled());
        if (IsLinkedToSwing()) {
            delayedSwingCollision = true;
        } else {
            isGrounded = false;
        }

        //if (nCurrentCollisions <= 0) {
        //    isGrounded = false;
        //}
    }

    // todo maybe this stuff should be on the swing platform's end?

    private void LinkToSwing(Transform swingPlatform) {
        // todo idk if i should be doing if checks in here, just terrible code
        transform.parent = swingPlatform;
        isGrounded = true;
    }

    private bool IsLinkedToSwing() {
        return gameObject.transform.parent != null &&
            transform.parent.GetComponent<SwingPlatform>() != null &&
            isGrounded;
    }

    private void UnlinkFromSwing() {
        if (transform.parent != null &&
                transform.parent.GetComponent<SwingPlatform>() != null) {
            rb.velocity += GetVelocityOfGround(transform.parent.gameObject); // todo maybe have special case for swing, boosting player unnaturally if they jump within the last part of the swing
            transform.parent = null;
            isGrounded = false;
        }
        delayedSwingCollision = false;
    }

    private bool ShouldUnlinkFromSwing() {
        return delayedSwingCollision && (
            !WithinBoundsX(playerCollider, transform.parent.GetComponent<Collider2D>()) || // todo maybe just try with collision box above the platform like in the video?
            //!WithinBoundsY(transform, collision.transform, 0.1f) ||
            IsAbove(transform.parent.gameObject, gameObject) ||
            grapple.isEnabled() ||
            jumpCooldown > 0);
    }

    // todo move these to some utils class

    // returns true if a is within bounds of b on x-axis
    bool WithinBoundsX(Collider2D a, Collider2D b, float wiggleRoom = 0f) {
        float aMin = a.bounds.min.x - wiggleRoom;
        float aMax = a.bounds.max.x + wiggleRoom;
        float bMin = b.bounds.min.x;
        float bMax = b.bounds.max.x;
        return aMax > bMin && aMin < bMax;
    }

    // returns true if t1 is within y bounds of t2
    bool WithinBoundsY(Transform t1, Transform t2, float wiggleRoom = 0f) {
        return t1.position.y + (t1.localScale.y / 2) + wiggleRoom > t2.position.y - (t2.localScale.y / 2) &&
            t1.position.y - (t1.localScale.y / 2) - wiggleRoom < t2.position.y + (t2.localScale.y / 2);
    }

    // returns true if obj1 is above obj2
    // requires both objects to have Collider2D's (todo maybe make params Collider2D so don't have to specify this)
    bool IsAbove(GameObject obj1, GameObject obj2) {
        return obj1.GetComponent<Collider2D>().bounds.min.y > obj2.GetComponent<Collider2D>().bounds.max.y;
    }

    public void Jump() {
        UnlinkFromSwing(); // todo is this necessary?
        jumpCooldown = jumpCooldownDuration;
        grapple.OnJump(); // todo note setting canTransformGrapple=true when doing any jump
        isGrounded = false;
        // todo theres a better way than if else / switch
        float velocityX = 0f;
        switch (hitWallNormal) {
            case > 0:
                velocityX = wallJumpSpeed; 
                break;
            case < 0:  
                velocityX = -wallJumpSpeed;
                break;
            case 0:
                velocityX = rb.velocity.x + (moveX * forwardAirSpeed * Time.deltaTime);
                break;
        }
        float velocityY = rb.velocity.y + jumpSpeed;
        // todo should i add this?
        //if (velocityOfGround != Vector2.zero) {
        //    velocityX = velocityOfGround.x + rb.velocity.x + (moveX * forwardAirSpeed * Time.deltaTime);
        //    velocityY = velocityOfGround.y + jumpSpeed;
        //}
        // todo might want to ignore reverse x movement after wall jump for a few seconds?
        rb.velocity = new Vector2(velocityX, velocityY); // note not adding rb.velocity.y 
    }

    public void Bounce(Vector2 direction) {
        rb.velocity = direction * stdBounceSpeed;
    }

    public void FinishStun() {
        stunned = false;
        stunCnt = 0;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void Stun(float duration = stunDuration, Vector2 collisionNormal = new Vector2()) {
        stunned = true;
        stunCnt = duration;
        rb.velocity = collisionNormal * knockbackSpeed;
        GetComponent<SpriteRenderer>().color = Color.cyan;
        grapple.StopGrappling(); // todo currently this is causing no grapple animation to play
    }
}