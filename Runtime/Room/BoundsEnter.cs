using System;
using UnityEngine;

using static Utilities.Layer;
using static Utilities.Vector;

[Obsolete("Use RoomBound instead")]
public class BoundsEnter : MonoBehaviour 
{
    CameraControls camControls;
    public Room room;

    void Start()
    {
        camControls = Camera.main.GetComponent<CameraControls>();
    }
    
    void OnTriggerEnter2D(Collider2D collider) {
        if (LayerEquals(collider.gameObject.layer, PLAYER)) {
            camControls.room.Disable();
            Vector2 playerNewPosition = collider.transform.position;
            room.Enable();

            Vector2 spawn = transform.Find("Spawn").position;
            camControls.ChangeRoom(room, spawn);
            collider.GetComponent<Health>().room = room;
            collider.GetComponent<Health>().room.spawn = spawn;

            Vector2 direction = GetCardinalDirection(collider.transform, transform);
            if (direction.x != 0) playerNewPosition.x = spawn.x; 
            else if (direction.y != 0) playerNewPosition.y = spawn.y;
            collider.transform.position = playerNewPosition;
        }
    }
}
