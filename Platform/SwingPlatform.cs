using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingPlatform : MonoBehaviour
{
    Swing swing;
    Vector2 startPosition;
    Rigidbody2D rb;

    void Start() {
        swing = GetComponentInParent<Swing>();
        rb = GetComponent<Rigidbody2D>();
        transform.rotation = Quaternion.identity;
    }

    void Update() {
        transform.rotation = Quaternion.identity;
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if (swing.activated && LayerMaskContains(swing.activatorLayers, collision.gameObject.layer)) {
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

    public Vector2 GetPosition() {
        return transform.position;
    }

    public void SetPosition(Vector2 position) {
        transform.position = position;
    }

    public RigidbodyType2D GetBodyType() {
        if (rb == null) rb = GetComponent<Rigidbody2D>(); // todo shouldnt have to do this
        return rb.bodyType;
    }

    public void SetBodyType(RigidbodyType2D bodyType) {
        if (rb == null) rb = GetComponent<Rigidbody2D>(); // todo shouldnt have to do this
        rb.bodyType = bodyType;
    }

    public void Stop() {
        rb.velocity = Vector2.zero;
    }
}
