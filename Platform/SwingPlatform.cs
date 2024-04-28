using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingPlatform : MonoBehaviour
{
    public Swing swing;
    public Rigidbody2D rb; // why does this needed to be added in the inspector?
    Vector2 prevPos;
    Vector2 velocity;
    bool broken;

    public void Init() {
        //swing = GetComponentInParent<Swing>();
        //rb = GetComponent<Rigidbody2D>();
        transform.rotation = Quaternion.identity;
        prevPos = rb.position;
        broken = false;
    }

    public void Reset() {
        broken = false;
        rb.constraints &= RigidbodyConstraints2D.FreezePositionY; // todo shouldn't have to do this...
    }

    void Update() {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        rb.rotation = 0;
        if (!broken) rb.velocity = velocity;
    }

    // physics done in FixedUpdate
    void FixedUpdate() {
        // velocity isn't set by parent's rotation, so have to set manually
        if (!broken) {
            velocity = (rb.position - prevPos) / Time.deltaTime;
            prevPos = rb.position;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if (swing.platformTouchActivated && LayerMaskContains(swing.activatorLayers, collision.gameObject.layer)) {
            swing.Activate();
        }
    }

    public void OnCollisionEnterWithGrapple() {
        if (swing.platformTouchActivated) {
            swing.Activate();
        }
    }

    // todo create static extension class for this, see https://discussions.unity.com/t/check-if-layer-is-in-layermask/16007/2
    bool LayerMaskContains(LayerMask mask, int layer) {
        return mask == (mask | (1 << layer));
    }

    public void Break() {
        broken = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
        rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
    }
}
