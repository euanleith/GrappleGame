using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using static Utilities.ColliderUtilities;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public GrapplingGun grapple;
    private Collider2D playerCollider;

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

    public float minWalkableAngle = 0.5f;
    public float hitWallNormal; // todo not great name - it's referring to the normal of the wall currently being collided with, if there is one, and 0 otherwise

    public bool isGrounded = false;
    public Vector2 velocityOfGround;

    // todo could have a Cooldown object and iterate through a list of them in Player.Update
    public const float jumpCooldownDuration = 0.1f;
    public float jumpCooldown = 0f;

    bool delayedSwingCollision = false;

    private List<PlayerMovementAction> actions; // only first which satisfies action.ShouldDo() will be performed

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

        actions = new List<PlayerMovementAction> {
            new GrappleMovementAction(this),
            new JumpAction(this),
            new WallClimbAction(this),
            new GroundMovementAction(this),
            new AirMovementAction(this)
        };
    }

    void Update() {
        UpdateJumpCooldown();

        actions.Where(action => action.ShouldDo()).First().Do();

        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
    }

    private void LateUpdate() {
        if (ShouldUnlinkFromSwing()) {
            UnlinkFromSwing();
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (grapple.isEnabled()) return;
        hitWallNormal = 0f;
        ContactPoint2D contact = collision.GetContact(0); // only need 1 contact point as player collider is box
        if (IsCollidingWithWall(contact)) {
            hitWallNormal = contact.normal.x;
        } else {
            LinkToGround(collision.transform);
            grapple.OnCollisionWithGround();
        }
    }

    private bool IsCollidingWithWall(ContactPoint2D contact) {
        return Math.Abs(contact.normal.x) > minWalkableAngle;
    }

    private bool IsCollidingWithGround(ContactPoint2D contact) {
        return contact.normal.y > minWalkableAngle;
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
        hitWallNormal = 0f;

        // todo there's other cases;
        //  on hit by enemy..?
        //  on land on swing then die?
        // if player is within horizontal bounds of platform and hasnt jumped/grappled

        if (IsLinkedToSwing()) {
            delayedSwingCollision = true;
        } else {
            isGrounded = false;
        }
    }

    // todo maybe this stuff should be on the swing platform's end?

    private void LinkToGround(Transform ground) {
        transform.parent = ground;
        isGrounded = true;
    }

    private bool IsLinkedToSwing() {
        return gameObject.transform.parent != null &&
            transform.parent.GetComponent<SwingPlatform>() != null &&
            isGrounded;
    }

    public void UnlinkFromSwing() {
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
            IsAbove(transform.parent.GetComponent<Collider2D>(), playerCollider) ||
            grapple.isEnabled() ||
            jumpCooldown > 0);
    }

    private void UpdateJumpCooldown() {
        if (jumpCooldown > 0) {
            jumpCooldown -= Time.deltaTime;
        }
    }

    public void Jump() {
        actions.Where(action => action is JumpAction).First().Do();
    }

    public void Bounce(Vector2 direction) {
        rb.velocity = direction * stdBounceSpeed;
    }

    public void Stun(Vector2 collisionNormal) {
        rb.velocity = collisionNormal * knockbackSpeed;
    }
}