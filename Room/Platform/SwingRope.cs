using System.Collections.Generic;
using UnityEngine;

using static Utilities.Layer;

// todo rename SwingRopeRenderer
// todo or maybe split into SwingRopeRenderer and SwingRope (the latter of which deals with the functioning of the rope, i.e. breaking, movement)
public class SwingRope : MonoBehaviour {
    public float width = 0.25f;

    Swing swing;
    public Transform pivot;
    public Transform platform;

    public LineRenderer lineRenderer; // todo why do these need to be added in the inspector?
    public EdgeCollider2D edgeCollider;

    public void Init(bool isCollidable) {
        swing = GetComponentInParent<Swing>();

        //lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = true;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        
        edgeCollider = GetComponent<EdgeCollider2D>();
        edgeCollider.enabled = true;
        edgeCollider.edgeRadius = 0;
        edgeCollider.isTrigger = !isCollidable;

        Render();
    }

    public void Reset() {
        lineRenderer.enabled = true;
        edgeCollider.enabled = true;
    }

    void FixedUpdate() {
        //Render();
    }

    void Render() {
        lineRenderer.SetPositions(new Vector3[] { pivot.localPosition, platform.localPosition });
        edgeCollider.SetPoints(new List<Vector2>() { pivot.localPosition, platform.localPosition });
    }

    public void Break() {
        lineRenderer.enabled = false;
        edgeCollider.enabled = false;
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        OnCollision(collision.gameObject);
    }

    public void OnTriggerEnter2D(Collider2D edgeCollider) {
        OnCollision(edgeCollider.gameObject);
    }

    void OnCollision(GameObject edgeCollider) {
        if (swing.ropeTouchActivated && LayerMaskContains(swing.activatorLayers, edgeCollider.layer)) {
            swing.Activate();
        }
        if (swing.breakable && LayerMaskContains(swing.breakerLayers, edgeCollider.layer)) { // todo PLAYER_ATTACK_LAYER) {
            swing.Break();
        }
    }

    public void OnCollisionEnterWithGrapple() {
        swing.Activate();
    }
}
