using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingRope : MonoBehaviour {
    public float width = 0.25f;

    Swing swing;
    Transform startTransform;
    Transform endTransform;

    LineRenderer lineRenderer;
    EdgeCollider2D collider;

    void Start() {
        swing = GetComponentInParent<Swing>();
        startTransform = swing.springJoint.transform;
        endTransform = swing.springJoint.connectedBody.transform;

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;

        collider = GetComponent<EdgeCollider2D>();
        collider.edgeRadius = width;
    }

    void Update() {
        Render();
    }

    void Render() {
        if (!swing.breakable || swing.springJoint.enabled) {
            lineRenderer.SetPositions(new Vector3[] { startTransform.localPosition, endTransform.localPosition });
            collider.SetPoints(new List<Vector2>() { startTransform.localPosition, endTransform.localPosition });
        } else {
            lineRenderer.enabled = false;
            collider.enabled = false;
        }
    }

    public void OnTriggerEnter2D(Collider2D collider) {
        if (swing.breakable && LayerMaskContains(swing.breakerLayers, collider.gameObject.layer)) { // todo PLAYER_ATTACK_LAYER) {
            swing.Break();
        }
    }

    // todo create static extension class for this, see https://discussions.unity.com/t/check-if-layer-is-in-layermask/16007/2
    bool LayerMaskContains(LayerMask mask, int layer) {
        return mask == (mask | (1 << layer));
    }
}
