using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement : MonoBehaviour
{
    public Vector2 speed = new Vector2(2,1);

    public abstract Vector2 Move(Vector2 direction, Vector2 position, Vector2 currentDecelVelocity, Rigidbody2D rb, Vector2 collisionNormal);

    public abstract void OnCollision(Collision2D collision, Rigidbody2D rb);
}
