using UnityEngine;

using static Utilities.Layer;

public class SpawnPoint : MonoBehaviour, RoomElement
{
    // todo maybe want to activate on player interact (e.g. hold up while colliding with this)

    public Room room;

    public void Init() {}
    public void Reset() {}
    public void Disable() {}

    void OnTriggerEnter2D(Collider2D collider) {
        if (LayerEqualsAny(collider.gameObject.layer, PLAYER)) {
            collider.gameObject.GetComponent<Health>().SetSpawnRoom(room);
            GetComponent<SpriteRenderer>().color = Color.green;
        }
    }
}
