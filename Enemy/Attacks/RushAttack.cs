using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushAttack: Attack
{
    public float rushSpeed = 4;

    Rigidbody2D rb;
    MovementController movementController;
    float direction;

    public void Start() {
        base.Start();
        this.rb = gameObject.GetComponentInParent<Rigidbody2D>();
        this.movementController = gameObject.GetComponentInParent<MovementController>();
    }

    public override void SetHitbox(Transform enemy, Transform player) {
        base.SetHitbox(enemy, player);
        direction = enemy.position.x < player.position.x ? 1 : -1;
    }

    public override void KeepAttacking() {
        // todo im not sure how to move without changing rb.transform.position, because otherwise it gets overwritten by movementcontroller
        //  is there a way for me to communicate with movementcontroller from here? could have global variable here and and in enemycombat, which enemy accesses each frame and sends to movementcontroller
        //  thats probably better and ill do it, but im leaving this for now since its easier
        // maybe i can add an AddMovement function to movementController?
        rb.transform.position = rb.transform.position+new Vector3(rushSpeed*direction*Time.deltaTime,0,0);
    }

    public override bool IsFinished() {
        if (movementController.collisionNormal != Vector2.zero) Debug.Log("finished");
        return movementController.collisionNormal != Vector2.zero;
    }
}
