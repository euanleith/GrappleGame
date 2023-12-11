using System;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed;
    public Transform minPos, maxPos;
    public new Camera camera;
    private float height, width;
    public float offset;

    void Start()
    {
        height = 2f * camera.orthographicSize;
        width = height * camera.aspect;
        Move(false);
    }

    void Update()
    {

    }

    void LateUpdate() {
        if (transform.position != player.position) {
            Move();
        }
    }

    void Move(bool lerp = true) {
        Vector3 targetPos = player.position;

        Vector3 camBounds = new Vector3(
            Mathf.Clamp(targetPos.x, minPos.position.x + (width / 2), maxPos.position.x - (width / 2)),
            Mathf.Clamp(targetPos.y, minPos.position.y + (height / 2), maxPos.position.y - (height / 2)),
            Mathf.Clamp(targetPos.z, minPos.position.z, maxPos.position.z)
        );

        Vector3 newPos = lerp ? Vector3.Lerp(transform.position, camBounds, smoothSpeed) : camBounds;
        transform.position = newPos;
    }
}