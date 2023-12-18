using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;

    [HideInInspector] public MovementController movementController;
    [HideInInspector] public CombatController combatController;
    [NonReorderable] public List<Attack> attacks; // todo NonReorderable avoids this bug: https://stackoverflow.com/questions/68812616/unity-public-fields-visible-in-inspector-overlap-eachother
    public Movement idleMovement;
    public Movement aggroMovement;

    public void Start()
    {
        movementController = GetComponent<MovementController>();
        combatController = GetComponent<CombatController>();
    }
}
