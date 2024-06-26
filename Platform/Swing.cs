using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour, RoomElement {

    // todo maybe make these some single data type visible in the inspector
    public bool platformTouchActivated = false; // todo maybe want just player on 'stand on' and just on 'player'
    public bool ropeTouchActivated = false;
    public LayerMask activatorLayers = 1 << 12; // Player layer

    public bool breakable = false;
    public LayerMask breakerLayers = 1 << 2; // todo PLAYER_ATTACK_LAYER (currently just using IgnoreRaycast layer)

    SwingPlatform platform;
    SwingRopeRotation ropeRotation;
    SwingRope rope;

    public void Init() {
        platform = GetComponentInChildren<SwingPlatform>();
        ropeRotation = GetComponentInChildren<SwingRopeRotation>();
        rope = GetComponentInChildren<SwingRope>();
        ropeRotation.Init(IsInitActivated());
        if (platform) platform.Init();
        rope.Init();
        Reset();
    }

    public void Reset() {
        ropeRotation.Reset(IsInitActivated());
        if (platform) platform.Reset();
        rope.Reset();
    }

    public void Disable() { }

    public void Activate() {
        ropeRotation.Activate();
        // todo stop at some point? maybe when back at startPos?
    }

    // todo currently on break, collides with deathplatforms off screen
    public void Break() {
        ropeRotation.Break();
        platform.Break();
        rope.Break();
    }

    bool IsInitActivated() {
        return !(platformTouchActivated || ropeTouchActivated);
    }
}
