using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// todo rename SwingRopeRenderer
// todo or maybe split into SwingRopeRenderer and SwingRope (the latter of which deals with the functioning of the rope, i.e. breaking, movement)
public class SwingRope : MonoBehaviour {
    public float width = 0.25f;

    Swing swing;
    // start and end transform should be pivot and platform respectively in parent swing, and swapped for any child swings,
    // for more info see https://team-kbscl1g1am1g.atlassian.net/browse/GGBT-69
    public Transform pivot;
    public Transform platform;

    public LineRenderer lineRenderer; // todo why do these need to be added in the inspector?
    new public EdgeCollider2D collider;

    public void Init(bool isCollidable) {
        swing = GetComponentInParent<Swing>();

        //lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = true;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        
        //collider = GetComponent<EdgeCollider2D>();
        collider.enabled = true;
        collider.edgeRadius = 0;
        collider.isTrigger = !isCollidable;

        Render();
    }

    public void Reset() {
        lineRenderer.enabled = true;
        collider.enabled = true;
    }

    void FixedUpdate() {
        //Render();
    }

    void Render() {
        lineRenderer.SetPositions(new Vector3[] { pivot.localPosition, platform.localPosition });
        collider.SetPoints(new List<Vector2>() { pivot.localPosition, platform.localPosition });
    }

    public void Break() {
        lineRenderer.enabled = false;
        collider.enabled = false;
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        OnCollision(collision.gameObject);
    }

    public void OnTriggerEnter2D(Collider2D collider) {
        OnCollision(collider.gameObject);
    }

    void OnCollision(GameObject collider) {
        if (swing.ropeTouchActivated && LayerMaskContains(swing.activatorLayers, collider.layer)) {
            swing.Activate();
        }
        if (swing.breakable && LayerMaskContains(swing.breakerLayers, collider.layer)) { // todo PLAYER_ATTACK_LAYER) {
            swing.Break();
        }
    }

    // todo create static extension class for this, see https://discussions.unity.com/t/check-if-layer-is-in-layermask/16007/2
    bool LayerMaskContains(LayerMask mask, int layer) {
        return mask == (mask | (1 << layer));
    }
}
