using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingPlatform : MonoBehaviour
{
    Swing swing;
    Rigidbody2D rb;
    Vector2 prevPos;
    Vector2 velocity;

    public void Init() {
        swing = GetComponentInParent<Swing>();
        rb = GetComponent<Rigidbody2D>();
        //rb.bodyType = RigidbodyType2D.Static;
        transform.rotation = Quaternion.identity;
        prevPos = rb.position;
    }

    void Update() {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        rb.rotation = 0;
        rb.velocity = velocity;
    }

    // physics done in FixedUpdate
    void FixedUpdate() {
        // velocity isn't set by parent's rotation, so have to set manually
        velocity = (rb.position - prevPos) / Time.deltaTime;
        prevPos = rb.position;
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if (!swing.initActivated && LayerMaskContains(swing.activatorLayers, collision.gameObject.layer)) {
            swing.Activate();
        }
    }


    public void OnCollisionEnterWithGrapple() {
        swing.Activate();
    }

    // todo create static extension class for this, see https://discussions.unity.com/t/check-if-layer-is-in-layermask/16007/2
    bool LayerMaskContains(LayerMask mask, int layer) {
        return mask == (mask | (1 << layer));
    }

    public RigidbodyType2D GetBodyType() {
        if (rb == null) rb = GetComponent<Rigidbody2D>(); // todo shouldnt have to do this
        return rb.bodyType;
    }

    public void SetBodyType(RigidbodyType2D bodyType) {
        if (rb == null) rb = GetComponent<Rigidbody2D>(); // todo shouldnt have to do this
        //rb.bodyType = bodyType;
    }

    public void Stop() {
        rb.velocity = Vector2.zero;
    }
}
