using UnityEngine;

using static Utils.Layers;

public class GrappleablePlatform : MonoBehaviour
{
    public float speed = 2f;
    public bool reusable = false;

    Rigidbody2D rb;
    bool used = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // todo integration with ropes
    // todo these should reset with room
    // todo should i have a variation which only ever moves in one direction, regardless of grapple direction?
    public void OnCollisionEnterWithGrapple(Vector2 direction) {
        Debug.Log(direction); // todo why does this give 0.71 for diagonals instead of 0.5?
        if (!used || reusable) {
            rb.velocity = -direction * speed;
            used = true;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if (LayerEqualsAny(collision.gameObject.layer, PLAYER)) {
            rb.velocity = Vector2.zero;
        }
    }
}
