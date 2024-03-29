using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// todo rename SwingRopeRenderer
// todo or maybe split into SwingRopeRenderer and SwingRope (the latter of which deals with the functioning of the rope, i.e. breaking, movement)
public class SwingRope : MonoBehaviour {
    public float width = 0.25f;

    Swing swing;
    public Transform startTransform;
    public Transform endTransform;

    LineRenderer lineRenderer;
    new EdgeCollider2D collider;

    void Start() {
        swing = GetComponentInParent<Swing>();

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
        //if (!swing.breakable || swing.springJoint.enabled) {
            Vector3 endTransformLocalPosition = endTransform.position - endTransform.parent.position + endTransform.parent.localPosition; // position determined by rotation of parent
            lineRenderer.SetPositions(new Vector3[] { startTransform.localPosition, endTransformLocalPosition });
            collider.SetPoints(new List<Vector2>() { startTransform.localPosition, endTransformLocalPosition });
        //} else {
            //lineRenderer.enabled = false;
        //    collider.enabled = false;
        //}
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
