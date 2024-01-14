using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement : MonoBehaviour
{
    public Vector2 speed = new Vector2(2,1);

    public abstract Vector2 Move(ref Vector2 direction, Transform transform, Vector2 currentDecelVelocity, Vector2 collisionNormal);
    
    // todo maybe make default versions of these? or do they even need to be variable?
    public abstract Vector2 OnCollision(Collision2D collision);
    
    public abstract Vector2 OnHit(Collider2D collider, Transform transform);
}
