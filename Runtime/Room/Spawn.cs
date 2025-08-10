using UnityEngine;

using static Utilities.Vector;

public class Spawn : MonoBehaviour 
{
    private Vector3 cardinalDirection;
    public Room room;

    public void Start() {
        cardinalDirection = GetCardinalDirection(transform.parent, transform);
        // todo can i get room just from location?
    }

    public Vector3 GetDirection() {
        return cardinalDirection;
    }

    public Vector2 GetPosition() {
        return transform.position;
    }

    public Room GetRoom() {
        return room;
    }
}