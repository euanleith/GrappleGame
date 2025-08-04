using UnityEngine;

public class BoundsEnter : MonoBehaviour 
{
    CameraControls camControls;
    public Room room;

    void Start()
    {
        camControls = Camera.main.GetComponent<CameraControls>();
    }
    
    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.layer == 12) { // player
            camControls.room.Disable();
            Vector2 playerNewPosition = collider.transform.position;
            room.Enable();

            Vector2 spawn = transform.Find("Spawn").position;
            camControls.room = room;
            camControls.room.spawn = spawn;
            collider.GetComponent<Health>().room = room;
            collider.GetComponent<Health>().room.spawn = spawn;

            // todo nope, GetDirection is wrong
            Vector2 direction = GetCardinalDirection(collider.transform, transform);
            Debug.Log(direction);
            Debug.Log("player current position: " + playerNewPosition);
            Debug.Log("new spawn position: " + transform.Find("Spawn").position);
            if (direction.x != 0) playerNewPosition.x = spawn.x; 
            else if (direction.y != 0) playerNewPosition.y = spawn.y;
            Debug.Log("new player position: " + playerNewPosition);
            collider.transform.position = playerNewPosition;
        }
    }

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
