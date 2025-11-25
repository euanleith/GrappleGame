using UnityEngine;

public static class Controls {

    public static bool IsPhysicsGrappling() {
        return Input.GetButton("Fire1");
    }

    public static bool IsTransformGrappling() {
        return Input.GetButton("Fire2");
    }

    public static bool IsGrappling() {
        return IsPhysicsGrappling() || IsTransformGrappling();
    }

    public static bool IsAttacking() {
        return Input.GetButton("Fire3");
    }

    public static bool IsJumping() {
        return Input.GetButtonDown("Jump");
    }

    public static float GetMovementX() {
        return Input.GetAxis("Horizontal");
    }

    public static float GetMovementY() {
        return Input.GetAxis("Vertical");
    }

    public static Vector2 GetMovement() {
        return new Vector2(GetMovementX(), GetMovementY());
    }

}