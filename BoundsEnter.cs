using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsEnter : MonoBehaviour 
{
    // todo when entering a room from below, could start with some upwards speed
    //  respawns when entering from below and there's no platform to land on are weird; maybe should respawn at end of previous room?

    CameraControls camControls;
    public Room room;

    void Start()
    {
        camControls = Camera.main.GetComponent<CameraControls>();
    }
    
    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.name == "Player") {
            camControls.room.Disable();
            room.Enable();

            // todo clean up
            camControls.room = room;
            Vector2 spawn = transform.Find("Spawn").position;
            camControls.room.spawn = spawn;
            collision.transform.position = spawn;
            collision.gameObject.GetComponent<Health>().room = room;
            collision.gameObject.GetComponent<Health>().room.spawn = spawn;
        }
    }
}
