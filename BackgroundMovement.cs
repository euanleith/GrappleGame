using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMovement : MonoBehaviour 
{
    public new Camera camera;
    public float parallaxEffect = 1f;

    private Vector3 prevCameraPos;
    private bool start = false;

    void Start() {
        prevCameraPos = camera.transform.position;
    }

    void Update() {
        Vector3 cameraVel = camera.transform.position - prevCameraPos;
        if (cameraVel == Vector3.zero) start = true; // todo camera doesn't start in the room, so it changes position, and so has velocity, which messes up the initial transform.position here
        if (start) {
            transform.Translate(cameraVel * parallaxEffect, camera.transform);
        }
        prevCameraPos = camera.transform.position;
    }
}
