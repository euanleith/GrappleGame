using System;
using System.Collections;
using UnityEngine;


// todo rename PlayerMovement, or just Movement
public class PlayerControls : MonoBehaviour
{
    // todo order these

    public Rigidbody2D rb;
    public new Camera camera;
    public GrapplingGun grapple;

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


    public float moveX;
    public float moveY;

    public float hitWallNormal; // todo not great name - it's reffering to the normal of the wall currently being collided with, if there is one, and 0 otherwise
    public float minWallAngle = 0.5f;

    BoxCollider2D boxColliderPlayer;
    int layerMaskGround;
    float heightTestPlayer; // todo name

    private Vector2 previousPos;

    public Vector2 velocityOfGround;

    public bool isGrounded = false;

    public float minSlope = 0.5f; // todo might need to change this for slopes

    Collider2D lastGroundCollision; // need this since OnCollisionExit2D doesn't contain collision info of the exited collision

    public bool stunned = false;
    public const float stunDuration = 1f;
    float stunCnt = 0f;

    void Start()
    {
        previousPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        boxColliderPlayer = GetComponent<BoxCollider2D>();
        heightTestPlayer = Mathf.Sqrt(Mathf.Sqrt(boxColliderPlayer.size.x)*2);
        layerMaskGround = LayerMask.GetMask("Ground");
        StartCoroutine(SnapToGround());
    }

    void Update()
    {
        if (stunCnt > 0) {
            stunCnt -= Time.deltaTime;
            return;
        } else if (stunned) FinishStun();

        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");

        if (!(Input.GetButton("Fire1") || Input.GetButton("Fire2"))) // todo if not grappling
        {
            // todo would making this a switch make it clearer?
            if ((isGrounded || (hitWallNormal != 0)) && Input.GetButtonDown("Jump")) { // jumping
                Jump();
            }
            else if (isGrounded && 
                    hitWallNormal != 0 && 
                    rb.velocity.x * hitWallNormal < 0)
            {
                rb.velocity = new Vector2(0, wallClimbSpeed);
            }
            else if (isGrounded) { // moving along ground
                if (moveX != 0) {
                    if (moveX * velocityOfGround.x > 0) { // not add velocityOfGround.x when moveX is in the opposite direction
                    rb.velocity = new Vector2((moveX * groundSpeed) + velocityOfGround.x, velocityOfGround.y);
                    } else {
                        rb.velocity = new Vector2((moveX * groundSpeed), velocityOfGround.y);
                    }
                } 
                else {
                    rb.velocity = velocityOfGround; 
                }
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
    }

    IEnumerator SnapToGround()
    {
        while (isGrounded) {
            //RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(2,2), 0, Vector2.down);
            RaycastHit2D hit = Physics2D.Raycast(boxColliderPlayer.bounds.center, Vector2.down, layerMaskGround);
            float distanceToGround = transform.position.y - hit.point.y;
            transform.position += Vector3.up * (0.5f - distanceToGround);

            yield return new WaitForFixedUpdate();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        hitWallNormal = 0f;
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Math.Abs(contact.normal.x) > minWallAngle)
            {
                hitWallNormal = contact.normal.x;
                break;
            } else {
                if (contact.normal.y > minSlope) {
                    lastGroundCollision = contact.collider;
                    isGrounded = true;
                    grapple.OnCollisionWithGround();
                }
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision) {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Math.Abs(contact.normal.x) <= minWallAngle)
            {
                if (contact.normal.y > minSlope) {
                    velocityOfGround = GetVelocityOfGround(contact.collider.gameObject.transform); // todo would this not mean that it wouldn't be grounded if connected to a non ground platform, even if also connected to a ground platform?
                }
            }
        }
    }

    public Vector2 GetVelocityOfGround(Transform ground) {
        try {
            return ground.gameObject.GetComponent<Rigidbody2D>().velocity;
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
        hitWallNormal = 0f;
        // todo for slopes
        //RaycastHit2D hit = Physics2D.Raycast(boxColliderPlayer.bounds.center, Vector2.down, layerMaskGround);
        //float distanceToGround = transform.position.y - hit.point.y;
        //Debug.Log(distanceToGround);
        //if (distanceToGround > 3) isGrounded = false; // todo ew, also still doesnt work, but a good way to test SnapToGround by making sure isGrounded is true
        if (lastGroundCollision != null && collision.collider == lastGroundCollision) {
            isGrounded = false;
        }
    }

    public void Jump() {
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
        float velocityY = jumpSpeed;
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
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void Stun(float duration = stunDuration, Vector2 collisionNormal = new Vector2()) {
        stunCnt = duration;
        rb.velocity = collisionNormal * knockbackSpeed;
        GetComponent<SpriteRenderer>().color = Color.cyan;
        grapple.StopGrappling(); // todo currently this is causing no grapple animation to play
    }
}
