using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour, RoomElement
{
    // todo jumping while on these isn't working properly
    //  swinging is weird too
    // todo it loses momentum over time, i dont want that

    // todo move all this to parent Swing object? then can have only one script etc

    public bool playerActivated = false; // todo might want it to be activated by other things to? also maybe want just player on stand on and just on player grapple

    Rigidbody2D platform; // todo currently this is dynamic, should really be kinematic with manual collision detection
    Vector2 platformStartPos;

    public void Init()
    {
        platform = GetComponent<Rigidbody2D>();
        platformStartPos = platform.position;
        Reset();
    }

    public void Reset() {
        // todo on start, platform moves to where the spring joint wants it. is there a way to move it to that point here so i dont have to manually get it right for every one?
        platform.position = platformStartPos;
        transform.parent.GetComponentInChildren<SpringJoint2D>().enabled = true;
        if (platform.bodyType != RigidbodyType2D.Static) platform.velocity = Vector2.zero;
        if (playerActivated) {
            platform.bodyType = RigidbodyType2D.Static;
        }
    }

    public void Disable() {}

    public void OnCollisionEnter2D(Collision2D collision) {
        if (playerActivated && collision.gameObject.layer == 12) { // Player layer 
            Activate();
        }
    }

    public void OnCollisionEnterWithGrapple() {
        if (playerActivated) {
            Activate();
        }
    }

    public void Activate() {
        platform.bodyType = RigidbodyType2D.Dynamic;
        // todo stop at some point? maybe when back at startPos?
    }
}
