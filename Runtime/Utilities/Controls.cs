using UnityEngine;

public static class Controls {

    public static bool isPhysicsGrappling() {
        return Input.GetButton("Fire1");
    }

    public static bool isTransformGrappling() {
        return Input.GetButton("Fire2");
    }

    public static bool isGrappling() {
        return isPhysicsGrappling() || isTransformGrappling();
    }

    public static bool isJumping() {
        return Input.GetButtonDown("Jump");
    }

    public static float getPlayerVelocityX() {
        return Input.GetAxis("Horizontal");
    }

    public static float getPlayerVelocityY() {
        return Input.GetAxis("Vertical");
    }

    public static Vector2 getPlayerVelocity() {
        return new Vector2(getPlayerVelocityX(), getPlayerVelocityY());
    }

}