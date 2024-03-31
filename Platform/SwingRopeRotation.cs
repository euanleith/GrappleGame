using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * todo
 *  breaks after respawning
 *  breaks when player interacts with it
 *  'break' functionality not working
 *  figure out speed
 *  platform is still rotating
 */
public class SwingRopeRotation : MonoBehaviour
{
    public float speed = 1f; // todo maybe speed should be determined by angleRange, since currently it takes the same amount of time for different size ranges, meaning different speeds. or at least influenced by anglerange

    float angleRange;
    float timeOffset;

    void Start() {
        Transform child = transform.GetChild(0).transform;
        float length = Vector2.Distance(Vector2.zero, child.localPosition);

        // angle range calculated as the angle between the pivot-down vector and the starting pivot-platform vector
        angleRange = Vector2.SignedAngle(new Vector2(0, -length), child.localPosition);
        timeOffset = CalculateTimeOffset(angleRange);

        // move platform to directly below pivot and maintain angleOffset
        child.localPosition = new Vector2(0, -length);
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

    float Angle(Vector2 pos1, Vector2 pos2) {
        return Mathf.Atan2(pos2.y - pos2.x, pos1.y - pos1.x) * 180 / Mathf.PI;
    }
}
