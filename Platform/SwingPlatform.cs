using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingPlatform : MonoBehaviour
{
    Swing swing;
    Vector2 startPosition;
    Rigidbody2D rb;


    float maxAngleDeflection;
    float pendulumSpeed;
    float offset;

    void Start() {
        swing = GetComponentInParent<Swing>();

        SwingRopeMovement ropeMovement = GetComponentInParent<SwingRopeMovement>();
        maxAngleDeflection = ropeMovement.maxAngleDeflection;
        pendulumSpeed = ropeMovement.pendulumSpeed;
        offset = ropeMovement.offset;
    }

    void Update() {
        // pendulum rotation
        float angle = maxAngleDeflection * Mathf.Sin(offset * pendulumSpeed);
        transform.localRotation = Quaternion.Euler(0, 0, -angle);
        offset += Time.deltaTime;
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
}
