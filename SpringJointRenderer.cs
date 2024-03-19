using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringJointRenderer : MonoBehaviour
{
    public Transform StartTransform;
    public Transform EndTransform;
    public LineRenderer ropeLineRenderer;
    public EdgeCollider2D ropeCollider;
    private SpringJoint2D springJoint;
    public float width = 0.25f;
    public bool breakable = false;

    void Start() {
        ropeLineRenderer.startWidth = width;
        ropeLineRenderer.endWidth = width;
        ropeCollider.edgeRadius = width;
        springJoint = GetComponent<SpringJoint2D>();
    }

    void Update() {
        if (!breakable || springJoint.enabled) {
            ropeLineRenderer.enabled = true;
            ropeCollider.enabled = true;
            ropeLineRenderer.SetPositions(new Vector3[] { StartTransform.localPosition, EndTransform.localPosition });
            ropeCollider.SetPoints(new List<Vector2>() { StartTransform.localPosition, EndTransform.localPosition });
        } else {
            ropeLineRenderer.enabled = false;
            ropeCollider.enabled = false;
        }
    }
}
