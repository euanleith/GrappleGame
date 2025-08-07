using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * todo
 *  'break' functionality not working
 *  figure out speed
 */
public class SwingRopeRotation : MonoBehaviour
{
    public float speed = 1f; // todo maybe speed should be determined by angleRange, since currently it takes the same amount of time for different size ranges, meaning different speeds. or at least influenced by anglerange

    [HideInInspector] public float angleRange;
    float timeOffset;
    float startTimeOffset;

    public Rigidbody2D rb; // todo why does this have to be added in the inspector
    bool activated;
    bool broken = false;

    public void Init(bool activated) {
        //rb = GetComponent<Rigidbody2D>();
        this.activated = activated;

        Transform child = transform.GetChild(0).transform; // todo this should be defined more strictly in the editor
        float length = Vector2.Distance(Vector2.zero, child.localPosition);

        // angle range calculated as the angle between the pivot-down vector and the starting pivot-platform vector
        Vector2 downVector = new Vector2(0, -length);
        angleRange = Vector2.SignedAngle(downVector, child.localPosition);
        timeOffset = CalculateTimeOffset(angleRange, angleRange);
        startTimeOffset = timeOffset;

        // move platform to directly below pivot and maintain angleOffset
        //child.localPosition = new Vector2(0, -length);
    }

    public void Reset(bool activated) {
        this.activated = activated;
        broken = false;
        timeOffset = startTimeOffset;
        
    }

    public void Activate() {
        activated = true;
    }

    public void Break() {
        broken = true;
    }

    void FixedUpdate()
    {
        float angle = angleRange * Mathf.Sin(timeOffset * speed); // shm equation: https://en.wikipedia.org/wiki/Simple_harmonic_motion
        angle -= angleRange; // want rotation = 0 on start, so rotating through 0 -> angleRange*2 instead of -angleRange -> angleRange
        if (!broken) transform.rotation = Quaternion.Euler(0, 0, angle);
        if (activated) timeOffset += Time.deltaTime;
    }

    float CalculateTimeOffset(float angle, float angleRange) {
        return (Mathf.Asin(angle / angleRange)) / speed; // this is just a refactor of the shm equation
    }

    float Angle(Vector2 pos1, Vector2 pos2) {
        return Mathf.Atan2(pos2.y - pos2.x, pos1.y - pos1.x) * 180 / Mathf.PI;
    }
}
