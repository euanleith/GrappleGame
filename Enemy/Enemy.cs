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

    public virtual bool OnHit(int damage) {
        combatController.health -= damage;
        //GetComponent<SpriteRenderer>().color = Color.red;
        // todo get hit animation, and maybe bounce backwards too, and stun both them and player (maybe stun instead of hurt player, or maybe it depends on the enemy?)
        if (combatController.health <= 0) {
            Debug.Log("BLEGH!!!!");
            gameObject.SetActive(false);
            return false;
        } else {
            Debug.Log("ow :(");
            return true;
        }
    }
}
