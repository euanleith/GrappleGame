using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour, RoomElement {

    public bool activated = false; // todo maybe want just player on 'stand on' and just on 'player'
    public LayerMask activatorLayers = 1 << 12; // Player layer

    public bool breakable = false;
    public LayerMask breakerLayers = 1 << 2; // todo PLAYER_ATTACK_LAYER (currently just using IgnoreRaycast layer)

    SwingPlatform platform;
    Vector2 platformStartPos;

    public void Init() {
        platform = GetComponentInChildren<SwingPlatform>();
        platformStartPos = platform.GetPosition();
        Reset();
    }

    public void Reset() {
        // todo on start, platform moves to where the spring joint wants it. is there a way to move it to that point here so i dont have to manually get it right for every one?
        platform.SetPosition(platformStartPos);
        if (platform.GetBodyType() != RigidbodyType2D.Static) platform.Stop();
        if (activated) {
            platform.SetBodyType(RigidbodyType2D.Static);
        }
    }

    public void Disable() { }

    public void Activate() {
        platform.SetBodyType(RigidbodyType2D.Dynamic);
        // todo stop at some point? maybe when back at startPos?
    }

    // todo currently on break, collides with deathplatforms off screen
    public void Break() {
        //springJoint.enabled = false; // todo
    }
}
