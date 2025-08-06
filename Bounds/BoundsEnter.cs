using UnityEngine;

using static Utils.Layers;

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

    // todo move these to utils

    public Vector2 GetCardinalDirection(Transform a, Transform b) {
        Vector2 direction = a.position - b.position;
        Vector2 bLocal = b.InverseTransformDirection(direction);
        Vector2 bScale = b.lossyScale;
        Vector2 directionLocal = Divide(bLocal, bScale);

        return Mathf.Abs(directionLocal.x) > Mathf.Abs(directionLocal.y) ? 
            (directionLocal.x > 0 ? 
                Vector2.right : Vector2.left) :
            (directionLocal.y > 0 ? 
                Vector2.up : Vector2.down);
    }

    public Vector3 Divide(Vector3 a, Vector3 b) {
        return new Vector3(
            a.x / b.x,
            a.y / b.y,
            a.z / b.z
        );
    }
}
