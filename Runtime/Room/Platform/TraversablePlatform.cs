using System;
using UnityEngine;

[Obsolete("Use PlatformEffector2D instead")]
public class TraversablePlatform : MonoBehaviour, RoomElement
{

    public new Collider2D collider;
    public Transform player;
    bool traversing = false;
    new bool enabled = false;

    // if update runs before box collider processes the collision and player is falling, player could be underneath the platform, which would produce wrong output, so adding a buffer
    public float buffer = 0.1f; 

    void Start() {
        if (collider == null) {
            collider = GetComponentInParent<Collider2D>();
        }
    }

    public void Init() {
    }

    public void Reset() {
        enabled = true;
    }

    void Update()
    {
        if (enabled) {
            // todo this won't work for tilemaps. can i do like 'if player colliding with the top of the platform's collider'?
            if (player.position.y - player.localScale.y / 2 + buffer < transform.position.y + transform.localScale.y / 2) {
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

    public void OnCollisionEnter2D(Collision2D collision) {
        if (enabled && collision.GetContact(0).normal.y > 0.9f) {
            collider.isTrigger = true;
            traversing = false;
        }
    }
}
