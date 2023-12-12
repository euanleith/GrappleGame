using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // todo dont need to run when not in the room. also make sure they dont leave the room
    
    public Transform player;

    protected Vector2 startPos;
    protected Vector2 speed;
    protected Vector2 moveRange;

    [HideInInspector] public EnemyMovement movementController;
    [HideInInspector] public EnemyCombat combatController;

    public void Start()
    {
        speed = GetComponent<EnemyMovement>().speed;
        moveRange = GetComponent<EnemyMovement>().moveRange;
        startPos = transform.position;
        movementController = GetComponent<EnemyMovement>();
        combatController = GetComponent<EnemyCombat>();
    }
}
