using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour, Platform
{
    public bool playerActivated = false; // todo might want it to be activated by other things to?

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
        platform.velocity = Vector2.zero;
        if (playerActivated) {
            platform.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    public void Disable() {}

    public void OnCollisionEnter2D(Collision2D collision) {
        if (playerActivated && collision.gameObject.layer == 12) { // Player layer 
            platform.constraints &= ~RigidbodyConstraints2D.FreezePositionX & ~RigidbodyConstraints2D.FreezePositionY;
            // todo stop at some point? maybe when back at startPos?
        }
    }
}
