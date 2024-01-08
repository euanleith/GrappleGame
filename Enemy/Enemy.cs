using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;

    [HideInInspector] public MovementController movementController;
    [HideInInspector] public CombatController combatController;
    public List<Attack> attacks; 
    public Movement idleMovement;
    public Movement aggroMovement;

    public void Start()
    {
        movementController = GetComponent<MovementController>();
        combatController = GetComponent<CombatController>();
    }
}
