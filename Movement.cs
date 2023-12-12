using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement
{
    protected Vector2 speed;

    public Movement(Vector2 speed) {
        this.speed = speed;
    }

    public abstract Vector2 Move(Vector2 direction, Vector2 position, Vector2 currentDecelVelocity, Rigidbody2D rb);
}
