using UnityEngine;
using System;
using System.Collections.Generic;

public class GrapplingGun : MonoBehaviour
{
    // todo should there be a small cooldown for physics mode just to stop player spamming?
    // todo try changing dampingRatio...
    // todo if distance is too far for grapple, should still show attempt
    // todo to stop swinging about so much, maybe try increasing linear drag when grappling onto moving objects 

    public GrapplingRope grappleRope;
    public bool canTransformGrapple = true;

    // todo move this to a doc
    // should there be a limitation to the transform mode?
    //  limits the more powerful option, so forces player to decide when to use their charge
    //  adds something to keep track of, which increases stress and difficulty
    // so which limitation should we choose?
    //  cooldown
    //      add a time constraint to platforming, meaning;
    //          there could be situations where you make a jump hoping that your grapple refreshes in time
    //          but players could only go so fast
    //      a bit vague; not precise
    //  touched floor 
    //      precise limitation
    //      forces player to lose momentum to recharge, so there's a opportunity cost that needs to be calculated

    public float countdown;

    public LayerMask ignoreRaycast;

    private bool grappleToAll = false;
    public List<int> grapplableLayerNumbers;

    public Camera m_camera;

    public Transform gunHolder;
    public Transform firePoint;
    private SpringJoint2D springJoint;

    public float maxDistance;

    private enum LaunchType
    {
        Transform_Launch,
        Physics_Launch
    }

    private LaunchType launchType;
    private float grappleMoveSpeedY = 5f; // todo currently takes same amount of time regardless of distance i think, i want a static speed instead
    private float grappleMoveYMin = 0f;
    private float grappleMoveXMax = 0.5f;

    public Vector2 grapplePoint;
    public Vector2 grappleDistanceVector;

    private Health health;

    Vector2 relativeGrapplePoint = new Vector2(0, 0);

    public RaycastHit2D hit; // todo is there a better way? - using to keep track of enemy for processing collisions

    float distanceFromPivot;

    private void Start()
    {
        grappleRope.enabled = false;
        springJoint = gunHolder.GetComponent<SpringJoint2D>();
        springJoint.enabled = false;
        health = GetComponentInParent<Health>();
    }

    private void Update()
    {
        if (health.currentHealth <= 0 || GetComponentInParent<PlayerControls>().stunned) {
            springJoint.enabled = false;
            return;
        }
        if (Input.GetButtonDown("Fire1")) {
            SetGrapplePoint();
            UpdateGrapplePoint();
        }
        if (Input.GetButtonDown("Fire2") && canTransformGrapple)
        {
            SetGrapplePoint();
            if (!GetComponentInParent<PlayerControls>().isGrounded) {
                canTransformGrapple = false;
                gunHolder.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
            }
        }
        if ((Input.GetButton("Fire1") || Input.GetButton("Fire2")) && springJoint.enabled)
        {
            UpdateGrapplePoint();
            GunHolderMove();
        }
        if (Input.GetButtonUp("Fire1") || Input.GetButtonUp("Fire2"))
        {
            StopGrappling();
        }
    }

    void SetGrapplePoint()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 distanceVector = ClampDirection(horizontal, vertical);
        hit = Physics2D.Raycast(firePoint.position, distanceVector.normalized, Mathf.Infinity, ~ignoreRaycast); // todo is there a way to include telephone wires without them having box colliders? or could just exclude player from collision matrix.
        if (hit)
        {
            // note that hit.collider is the collided object, where hit.transform is the parent collider object
            if (grapplableLayerNumbers.Contains(hit.transform.gameObject.layer) || grappleToAll)
            {
                if (Vector2.Distance(hit.point, firePoint.position) <= maxDistance)
                {
                    grapplePoint = hit.point;
                    grappleDistanceVector = grapplePoint - (Vector2)gunHolder.position;
                    relativeGrapplePoint = hit.point - (Vector2)hit.transform.position;
                    grappleRope.enabled = true;

                    // todo surely there's a better way of doing this :')
                    if (hit.transform.gameObject.layer == 8) { // todo maybe also only if transform launch?
                        hit.transform.gameObject.GetComponent<Enemy>().OnCollisionEnterWithGrapple();
                    } else if (hit.transform.gameObject.layer == 20) {
                        hit.transform.gameObject.GetComponent<GrappleablePlatform>().OnCollisionEnterWithGrapple(distanceVector);
                    } else if (hit.collider.gameObject.layer == 21) {
                        hit.collider.gameObject.GetComponent<SwingPlatform>().OnCollisionEnterWithGrapple();
                    } else if (hit.collider.gameObject.GetComponent<SwingRope>() != null) {
                        distanceFromPivot = Vector2.Distance(hit.point, hit.collider.gameObject.GetComponent<SwingRope>().pivot.position);
                    }
                }
            }
        }
    }

    void UpdateGrapplePoint() {
        // take into account position & rotation of hit object

        // position
        //Vector2 relocatedGrapplePoint = (Vector2)hit.transform.position + relativeGrapplePoint;
        grapplePoint = (Vector2)hit.transform.position + relativeGrapplePoint;

        // if grapple swing rope
        // todo maybe i wouldn't need a separate section just for swing ropes if SwingRope was a child of pivot and rotated accordingly, though idk
        // actually even if i don't, i can just set the pivot to the object, and add a condition to set it to swingrope.pivot if its a swingrope
        if (hit.collider.gameObject.GetComponent<SwingRope>() != null) {
            Transform pivot = hit.collider.gameObject.GetComponent<SwingRope>().pivot;
            float angle = (pivot.eulerAngles.z-90) * Mathf.Deg2Rad; // need to subtract 90 degrees as rotating through 0->180 rather than -90->90, see https://github.com/euanleith/GrappleGame/commit/f7e0c2fde40b93b5c7b2481b6bb43c119cd27b39
            grapplePoint = new Vector2(pivot.position.x + distanceFromPivot * Mathf.Sin(angle), pivot.position.y - distanceFromPivot * Mathf.Cos(angle));
        }

        /*
        // rotation
        Vector2 centre = (Vector2)hit.transform.worldCenterOfMass;
        float rotation = hit.transform.eulerAngles.z;
        if (rotation < 0) rotation = 360-(-rotation%360); // convert from pos/neg degrees to just pos
        rotation = MathF.PI * rotation / 180f; // degrees to radians
        // calculate point along rotation - https://math.stackexchange.com/questions/3935956/calculate-the-new-position-of-a-point-after-rotating-it-around-another-point-2d
        grapplePoint.x = ((relocatedGrapplePoint.x - centre.x) * MathF.Cos(rotation)) - ((relocatedGrapplePoint.y - centre.y) * MathF.Sin(rotation)) + centre.x;
        grapplePoint.y = ((relocatedGrapplePoint.x - centre.x) * MathF.Sin(rotation)) + ((relocatedGrapplePoint.y - centre.y) * MathF.Cos(rotation)) + centre.y;

        // todo not moving along with grapple when rotating :( need to take some stuff from grapple? would be nice if just ran it every frame, but thats not working for some reason... maybe want to set grapple distance to the current, and only allow it to change with user input moving up and down?
        // works differently for transform than physics??
        */

        if (hit.transform.gameObject.GetComponent<Rigidbody2D>()) {
            springJoint.connectedBody = hit.transform.gameObject.GetComponent<Rigidbody2D>();
            springJoint.connectedAnchor = Vector2.zero;
        } else {
            springJoint.connectedAnchor = grapplePoint;
        }
    }

    public void Grapple()
    {
        if (Input.GetButton("Fire1")) {
            launchType = LaunchType.Physics_Launch;
        } else if (Input.GetButton("Fire2")) {
            launchType = LaunchType.Transform_Launch;
        }
        //springJoint.distance = launchType == LaunchType.Physics_Launch ? targetDistance : 0; // todo using current distance from point rather than targetDistance - i could potentially see this also being useful; maybe R1 button??
        Vector2 firePointDistance = firePoint.position - gunHolder.localPosition;
        Vector2 targetPos = grapplePoint - firePointDistance;
        float distance = Vector2.Distance(gunHolder.localPosition, targetPos);
        if (distance > maxDistance) {
            distance = maxDistance;
        }
        springJoint.distance = launchType == LaunchType.Physics_Launch ? distance : 0;
        if (launchType == LaunchType.Transform_Launch) {
            gunHolder.GetComponent<Rigidbody2D>().velocity = Vector2.zero; // todo should be doing GetPosition here and everywhere else
            gunHolder.GetComponent<Rigidbody2D>().gravityScale = 0; // todo need to turn this off once player hits the target, otherwise they can extend the grapple and swing without gravity
        }
        
        springJoint.enabled = true;
    }

    public void StopGrappling() {
        grappleRope.enabled = false;
        springJoint.enabled = false;
        springJoint.connectedBody = null;
        if (hit && hit.transform.gameObject.layer == 8) {
            hit.transform.gameObject.GetComponent<Enemy>().OnCollisionExitWithGrapple();
        }
        gunHolder.GetComponent<Rigidbody2D>().gravityScale = 1;
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, maxDistance);
        }
    }

    // reduce the scope of possible directions - https://forum.unity.com/threads/8-directional-movement-question.1087436/
    private Vector2 ClampDirection(float horizontal, float vertical) {
        int dirCount = 8;
        float angle = Mathf.Atan2(vertical, horizontal);
        float normalized = Mathf.Repeat(angle / (Mathf.PI*2f), 1f);
        angle = Mathf.Round(normalized * dirCount)*Mathf.PI*2f / dirCount;
        horizontal = (float)Math.Round(Mathf.Cos(angle),6);
        vertical = (float)Math.Round(Mathf.Sin(angle),6);
        return new Vector2(horizontal, vertical);
    }

    private void GunHolderMove() {
        float gunHolderVelocityY = Input.GetAxis("Vertical");
        float gunHolderVelocityX = Input.GetAxis("Horizontal");
        if (Math.Abs(gunHolderVelocityY) > grappleMoveYMin && Math.Abs(gunHolderVelocityX) < grappleMoveXMax) {
            springJoint.distance -= Time.deltaTime * (gunHolderVelocityY * grappleMoveSpeedY);
            if (springJoint.distance > maxDistance) {
                springJoint.distance = maxDistance;
            }
        }
    }

    public void OnCollisionWithGround() {
        canTransformGrapple = true;
        gunHolder.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void OnJump() {
        canTransformGrapple = true;
        gunHolder.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public bool isEnabled() {
        return springJoint.enabled;
    }

    public void Disable() {
        springJoint.enabled = false;
        grappleRope.enabled = false;
    }
}