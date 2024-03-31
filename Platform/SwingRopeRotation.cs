using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingRopeRotation : MonoBehaviour
{
    public float angleOffset = 90f;
    public float angleRange = 90.0f;
    public float speed = 1.0f;

    float timeOffset;

    void Start() {
        timeOffset = CalculateTimeOffset(angleOffset);
    }

    void Update()
    {
        float angle = angleRange * Mathf.Sin(timeOffset * speed); // shm equation: https://en.wikipedia.org/wiki/Simple_harmonic_motion
        transform.localRotation = Quaternion.Euler(0, 0, angle);
        timeOffset += Time.deltaTime;
    }

    float CalculateTimeOffset(float angle) {
        return (Mathf.Asin(angle / angleRange)) / speed; // this is just a refactor of the shm equation
    }
}
