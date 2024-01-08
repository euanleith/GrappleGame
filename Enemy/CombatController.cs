using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public int health = 1;
    public enum State
    {
        windup,
        attacking,
        cooldown, 
        idle
    }
    public State state;

    float countdown;

    Transform transform;
    Transform player;
    [HideInInspector] public List<Attack> attacks;
    Attack currentAttack; // todo could be int index to save a bit of space
    
    public void Start() {
        state = State.cooldown;
        transform = gameObject.transform;
        player = GetComponent<Enemy>().player;
        attacks = GetComponent<Enemy>().attacks;
        currentAttack = attacks[0]; 
        // todo add functionality for multiple attacks. might need DecideAttack() in Enemy, since deciding which attack to use might not be general to all enemies
        //  another option could be to have a CanAttack function in each attack which runs on cooldown instead if checking if within minAttackRange. Then if one attack CanAttack, that becomes the currentAttack. Otherwise if there's multiple, choose randomly between them?
        //      although sometimes i might want to prioritise one attack over another... i guess i could add a priority int to each Attack, but ew. 
        //          or i could use the order of attacks, but that's a bit unclear
        //          and this prioritisation might be situational, which requires logic
        //      i still could have subclasses of Enemy, i just want to avoid it if i can, since every other element of each enemy type is defined in the editor
        //          i guess i could also have a separate attack decider class, which is either random, or defined by me in the editor using the existing attacks
        //  how do other games do it?
    }


    public void FixedUpdate()
    {
        switch (state)
        {
            case State.idle:
                if (Math.Abs(transform.position.x - player.transform.position.x) < currentAttack.minAttackRange.x &&
                    Math.Abs(transform.position.y - player.transform.position.y) < currentAttack.minAttackRange.y)
                {    
                    countdown = currentAttack.windupDuration;
                    GetComponent<SpriteRenderer>().color = Color.red;
                    state = State.windup;
                    currentAttack.SetHitbox(transform, player);
                }
                break;
            case State.cooldown:
                if (countdown <= 0) {
                    state = State.idle;
                }
                break;
            case State.attacking:
                if (!currentAttack.IsFinished()) 
                {
                    currentAttack.KeepAttacking();
                } 
                else
                {
                    countdown = currentAttack.cooldownDuration;
                    currentAttack.Stop();
                    state = State.cooldown;
                }
                break;
            case State.windup:
                if (countdown <= 0)
                {
                    // todo add some function to determine which attack to use
                    currentAttack.StartAttacking();
                    GetComponent<SpriteRenderer>().color = Color.white;
                    state = State.attacking;
                }
                break;
        }   
        if (countdown > 0) {
            countdown -= Time.deltaTime;
        }
    }

    // todo maybe move this elsewhere? Enemy? since would be nice to call this AttackController
    public void GetHit(int damage) {
        health -= damage;
        //GetComponent<SpriteRenderer>().color = Color.red;
        // todo get hit animation, and maybe bounce backwards too, and stun both them and player (maybe stun instead of hurt player, or maybe it depends on the enemy?)
        if (health <= 0) {
            Debug.Log("BLEGH!!!!");
            gameObject.SetActive(false);
        } else {
            Debug.Log("ow :(");
        }
    }

    public int GetDamage() {
        return currentAttack.damage;
    }
}
