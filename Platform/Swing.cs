using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour, Platform
{
    Rigidbody2D platform;
    Vector2 platformStartPos;
    SpringJoint2D swing;

    public void Init()
    {
        platform = gameObject.transform.Find("Platform").GetComponent<Rigidbody2D>();
        platformStartPos = platform.position;
        swing = gameObject.transform.Find("Pivot").GetComponent<SpringJoint2D>();
    }

    public void Reset() {
        swing.enabled = true;
        // todo platform jumps a bit on reset, maybe because of sprint joint?
        //  maybe i could manually reset the position of the connected point?
        platform.position = platformStartPos; 
        platform.velocity = Vector2.zero;
    }

    public void Disable() {
        swing.enabled = false;
    }
}
