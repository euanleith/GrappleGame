using UnityEngine;

using static Utilities.Layer;
using static Utilities.Vector;

public class RoomConnector : MonoBehaviour {
    private CameraControls camControls;
    private Spawn[] spawns;

    void Start() {
        camControls = Camera.main.GetComponent<CameraControls>();
        spawns = GetComponentsInChildren<Spawn>();
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (LayerEquals(collider.gameObject.layer, PLAYER)) {
            Vector2 currentPlayerPosition = collider.transform.position;
            Vector2 cardinalDirection = GetCardinalDirection(collider.transform, transform);
            Spawn newSpawn = GetNewSpawn(cardinalDirection);

            camControls.ChangeRoom(newSpawn.GetRoom(), newSpawn.GetPosition()); // todo move to Somewhere.ChangeRoom()

            collider.GetComponent<PlayerHealth>().room = newSpawn.GetRoom(); // todo move to Somewhere.ChangeRoom()
            collider.GetComponent<PlayerHealth>().room.spawn = newSpawn.GetPosition(); // todo move to Somewhere.ChangeRoom()

            collider.transform.position = GetNewPlayerPosition(currentPlayerPosition, collider.transform, cardinalDirection);
        }
    }

    private Spawn GetNewSpawn(Vector2 cardinalDirection) {
        foreach (Spawn spawn in spawns) {
            if (spawn.GetDirection().Equals(cardinalDirection)) {
                return spawn;
            }
        }
        throw new MissingComponentException("No Spawn object exists in direction " + cardinalDirection);
    }

    private Vector2 GetNewPlayerPosition(Vector3 currentPlayerPosition, Transform player, Vector2 cardinalDirection) {
        Vector3 magnitude = (player.transform.localScale * 1.5f)
            + transform.localScale;
        Vector3 distance = cardinalDirection.x != 0 ?
            (cardinalDirection.x > 0 ?
                Vector2.left * magnitude.x:
                Vector2.right * magnitude.x) :
            (cardinalDirection.y > 0 ?
                Vector2.down * magnitude.y :
                Vector2.up * magnitude.y);
        return currentPlayerPosition + distance;
    }
}