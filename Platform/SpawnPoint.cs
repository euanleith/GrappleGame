using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour, RoomElement
{
    // todo maybe want to activate on player interact (e.g. hold up while colliding with this)

    public Room room;

    public void Init() {}
    public void Reset() {}
    public void Disable() {}

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.layer == 12) { // Player layer
            collider.gameObject.GetComponent<Health>().SetSpawnRoom(room);
            GetComponent<SpriteRenderer>().color = Color.green;
        }
    }
}
