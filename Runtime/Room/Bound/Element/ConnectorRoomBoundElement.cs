using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using static Utilities.Layer;
using static Utilities.Vector;

public class ConnectorRoomBoundElement : RoomBoundElement {

    [SerializeField][ReadOnly]
    private ConnectorRoomBoundElement connectedElement;

    [SerializeField][ReadOnly]
    private Spawn spawn;

    private CameraControls camControls;
    private bool associatedWithSpawn;

    new private void Reset() {
        base.Reset();
        if (this == null) return;
        GetComponent<SpriteRenderer>().color = Color.green;
        gameObject.layer = LayerToInt(TRANSITION); // todo idk if this is necessary, existing next rooms are all this layer though; maybe rename to ROOM_BOUND_CONNECTOR
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnEnable() {
        associatedWithSpawn = true;
    }

    new private void Update() {
        base.Update();
#if UNITY_EDITOR
        if (!Application.isPlaying) {
            ConnectorRoomBoundElement[] otherRoomConnectorCollisions = GetElementCollisions()
                .Where(hit => IsSameType(hit) && !InSameRoom(hit))
                .Cast<ConnectorRoomBoundElement>()
                .ToArray();
            if (otherRoomConnectorCollisions.Length == 0) {
                GetRoom().Remove(this); // todo don't delete if the two rooms are still colliding, just move&/resize this one
                connectedElement = null;
            } else if (otherRoomConnectorCollisions.Length == 1) {
                connectedElement = otherRoomConnectorCollisions[0];
                if (spawn == null && associatedWithSpawn) {
                    Debug.LogError($"Couldn't find Spawn to associate with Connector {GetName()}", this);
                    associatedWithSpawn = false;
                }
            } else {
                Debug.LogError($"Connector {GetName()} has multiple connected elements", this);
            }
        }
#endif
    }

    void Start() {
        camControls = Camera.main.GetComponent<CameraControls>();
    }

    public void SetSpawn(Spawn spawn) {
        this.spawn = spawn;
        associatedWithSpawn = true;
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (LayerEquals(collider.gameObject.layer, PLAYER)) {
            if (spawn == null) {
                Debug.LogError($"No Spawn associated with ConnectorRoomBoundElement {GetName()}", this);
            } else {
                Collider2D player = collider;
                Vector2 currentPlayerPosition = player.transform.position;
                Vector2 cardinalDirection = GetCardinalDirection(player.transform, transform);

                camControls.ChangeRoom(spawn.GetRoom(), spawn.GetPosition()); // todo move to Somewhere.ChangeRoom()

                player.GetComponent<PlayerHealth>().room = spawn.GetRoom(); // todo move to Somewhere.ChangeRoom()
                player.GetComponent<PlayerHealth>().room.spawn = spawn.GetPosition(); // todo move to Somewhere.ChangeRoom()

                player.transform.position = GetNewPlayerPosition(currentPlayerPosition, player.transform, cardinalDirection);
            }
        }
    }

    private const float SHIFT_WIGGLE_ROOM = 0.1f;
    private Vector2 GetNewPlayerPosition(Vector3 currentPlayerPosition, Transform player, Vector2 cardinalDirection) {
        Vector3 magnitude = Add(2*RoomBoundElementEditorHelper.WIDTH + SHIFT_WIGGLE_ROOM, player.transform.lossyScale);
        Vector3 distance = cardinalDirection.x != 0 ?
            (cardinalDirection.x > 0 ?
                Vector2.left * magnitude.x :
                Vector2.right * magnitude.x) :
            (cardinalDirection.y > 0 ?
                Vector2.down * magnitude.y :
                Vector2.up * magnitude.y);
        return currentPlayerPosition + distance;
    }

    public ConnectorRoomBoundElement GetConnectedElement() {
        return connectedElement;
    }
}