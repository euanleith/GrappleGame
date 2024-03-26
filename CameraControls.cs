using System;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed;
    public Room room;
    public new Camera camera;
    private float height, width;
    public float offset;

    void Start()
    {
        camera.aspect = 16f / 9f;
        height = 2f * camera.orthographicSize;
        width = height * (16f / 9f);
        room.Enable();
        Move(false); // todo maybe this is running before player is moved
    }

    void Update()
    {

    }

    void LateUpdate() {
        if (transform.position != player.position) {
            Move();
        }
    }

    public void Move(bool lerp = true) {
        Vector3 targetPos = player.position;
        Vector3 camBounds = new Vector3(
            Mathf.Clamp(targetPos.x, room.minPos.x + (width / 2), room.maxPos.x - (width / 2)),
            Mathf.Clamp(targetPos.y, room.minPos.y + (height / 2), room.maxPos.y - (height / 2)),
            0
        );

        Vector3 newPos = lerp ? Vector3.Lerp(transform.position, camBounds, smoothSpeed) : camBounds;
        transform.position = newPos;
    }
}