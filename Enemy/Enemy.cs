using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, RoomElement
{
    public Transform player;

    [HideInInspector] public MovementController movementController;
    [HideInInspector] public CombatController combatController;
    public List<Attack> attacks; 
    public Movement idleMovement;
    public Movement aggroMovement;

    public void Init()
    {
        movementController = GetComponent<MovementController>();
        movementController.Init();
        combatController = GetComponent<CombatController>();
    }

    public virtual void Update() {}

    public virtual bool OnHit(int damage) {
        combatController.health -= damage;
        //GetComponent<SpriteRenderer>().color = Color.red;
        // todo get hit animation, and maybe bounce backwards too, and stun both them and player (maybe stun instead of hurt player, or maybe it depends on the enemy?)
        if (combatController.health <= 0) {
            Debug.Log("BLEGH!!!!");
            Disable();
            return false;
        } else {
            Debug.Log("ow :(");
            return true;
        }
    }

    public virtual void Disable() {
        gameObject.SetActive(false);
    }

    public virtual void Reset() {
        gameObject.SetActive(true);
        movementController.Reset();
        combatController.Reset();
    }

    public virtual void OnCollisionEnterWithGrapple() {
        movementController.OnCollisionEnterWithGrapple();
    }

    public virtual void OnCollisionExitWithGrapple() {
        movementController.OnCollisionExitWithGrapple();
    }
}
