using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingRopeRotation : MonoBehaviour
{
    public float maxAngleDeflection = 60.0f;
    public float speed = 1.0f;
    public float offset;

    void Update()
    {
        float angle = maxAngleDeflection * Mathf.Sin(offset * speed);
        transform.localRotation = Quaternion.Euler(0, 0, angle);
        offset += Time.deltaTime;
    }
}
