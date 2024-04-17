using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraversablePlatform : MonoBehaviour, RoomElement
{

    new BoxCollider2D collider;
    public Transform player;
    bool traversing = false;
    new bool enabled = false;

    // if update runs before box collider processes the collision and player is falling, player could be underneath the platform, which would produce wrong output, so adding a buffer
    public float buffer = 0.1f; 

    void Start() {
        collider = GetComponentInParent<BoxCollider2D>();   
    }

    public void Init() {
    }

    public void Reset() {
        enabled = true;
    }

    void Update()
    {
        if (enabled) {
            if (player.position.y - player.localScale.y/2 + buffer < transform.position.y + transform.localScale.y/2) {
                collider.isTrigger = true;
                traversing = false;
            } else if (!traversing) {
                collider.isTrigger = false;
            }
        }
    }

    public void Traverse() {
        collider.isTrigger = true;
        traversing = true;
    }

    public void Disable() {
        enabled = false;
    }
}
