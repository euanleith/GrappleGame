using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingPlatform : MonoBehaviour
{
    Swing swing;

    void Start() {
        swing = GetComponentInParent<Swing>();
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
