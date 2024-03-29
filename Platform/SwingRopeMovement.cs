using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// todo could move this to Pivot in inspector?
public class SwingRopeMovement : MonoBehaviour
{
    public float maxAngleDeflection = 60.0f;
    public float pendulumSpeed = 1.0f;
    public float offset;

    void Update()
    {
        float angle = maxAngleDeflection * Mathf.Sin(offset * pendulumSpeed);
        transform.localRotation = Quaternion.Euler(0, 0, angle);
        offset += Time.deltaTime;
    }
}
