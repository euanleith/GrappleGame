using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongEnemy : Enemy
{
    void Start()
    {
        base.Start();
        movementController.idleMovement = new PingPongMovement(movementController.speed, movementController.startPos, movementController.moveRange); 
        movementController.aggroMovement = new FollowPlayerMovement(movementController.speed, player);
    }

    void Update()
    {
        
    }
}
