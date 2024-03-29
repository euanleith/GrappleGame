using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour, RoomElement {
    // todo jumping while on these isn't working properly
    //  swinging is weird too
    // todo it loses momentum over time, i dont want that


    public bool activated = false; // todo maybe want just player on 'stand on' and just on 'player'
    public LayerMask activatorLayers = 1 << 12; // Player layer

    public bool breakable = false;
    public LayerMask breakerLayers = 1 << 2; // todo PLAYER_ATTACK_LAYER (currently just using IgnoreRaycast layer)

    [HideInInspector] public SpringJoint2D springJoint;
    Rigidbody2D platform;
    Vector2 platformStartPos;

    public void Init() {
        springJoint = GetComponentInChildren<SpringJoint2D>();
        platform = springJoint.connectedBody;
        platformStartPos = platform.position;
        Reset();
    }

    public void Reset() {
        // todo on start, platform moves to where the spring joint wants it. is there a way to move it to that point here so i dont have to manually get it right for every one?
        platform.position = platformStartPos;
        //springJoint.enabled = true;
        if (platform.bodyType != RigidbodyType2D.Static) platform.velocity = Vector2.zero;
        if (activated) {
            platform.bodyType = RigidbodyType2D.Static;
        }
    }

    public void Disable() { }

    public void Activate() {
        platform.bodyType = RigidbodyType2D.Dynamic;
        // todo stop at some point? maybe when back at startPos?
    }

    // todo currently on break, collides with deathplatforms off screen
    public void Break() {
        springJoint.enabled = false;
    }
}
