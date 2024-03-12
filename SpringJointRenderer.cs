using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringJointRenderer : MonoBehaviour
{
    public Transform StartTransform;
    public Transform EndTransform;
    public LineRenderer scriptLineRenderer;


    void Update() {
        // 0 and 1 are position indexes
        scriptLineRenderer.SetPosition(0, StartTransform.position);
        scriptLineRenderer.SetPosition(1, EndTransform.position);

    }
}
