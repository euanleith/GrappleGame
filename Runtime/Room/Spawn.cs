using UnityEngine;
using Utilities;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class Spawn : MonoBehaviour 
{
    private Vector3 cardinalDirection;
    private Room room;
    private bool associatedWithConnector;
    [SerializeField] private bool isDefaultSpawn;

    [SerializeField][ReadOnly]
    private ConnectorRoomBoundElement connector;

    private void Start() {
        cardinalDirection = Vector.GetCardinalDirection(transform.parent, transform);
    }

    private void OnValidate() {
        if (room == null) room = GetComponentInParent<Room>();
        room.UpdateDefaultSpawn(this);
    }

    private void OnEnable() {
        associatedWithConnector = true;
    }

    public bool IsDefaultSpawn() {
        return isDefaultSpawn;
    }

    public void RemoveAsDefaultSpawn() {
        isDefaultSpawn = false;
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

    private void Update() {
#if UNITY_EDITOR
        if (!Application.isPlaying) {
            ConnectorRoomBoundElement[] otherRoomConnectors = room.GetBounds().GetConnectedRoomsConnectors();
            ConnectorRoomBoundElement otherRoomConnector = Vector.GetClosest(this, otherRoomConnectors);

            if (otherRoomConnector == null && associatedWithConnector) {
                connector = null;
                Debug.LogError($"Couldn't find Connector to associate with Spawn {room.name}.{name}", this);
                associatedWithConnector = false;
            } else if (otherRoomConnector != null && !associatedWithConnector) {
                otherRoomConnector.SetSpawn(this);
                connector = otherRoomConnector;
                Debug.Log($"Associating Spawn {GetName()} with Connector {otherRoomConnector.GetName()}", this);
                associatedWithConnector = true;
            }
        }
#endif
    }

    public string GetName() {
        return $"{room.name}.{name}";
    }
}