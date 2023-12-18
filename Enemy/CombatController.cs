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
        cooldown
    }
    public State state;

    float countdown;

    Transform transform;
    Transform player;
    [HideInInspector] public List<Attack> attacks; // todo NonReorderable avoids this bug: https://stackoverflow.com/questions/68812616/unity-public-fields-visible-in-inspector-overlap-eachother
    Attack currentAttack;
    
    public void Start() {
        state = State.cooldown;
        transform = gameObject.transform;
        player = GetComponent<Enemy>().player;
        attacks = GetComponent<Enemy>().attacks;
        currentAttack = attacks[0]; 
        // todo add functionality for multiple attacks. might need DecideAttack() in Enemy, since deciding which attack to use might not be general to all enemies
        //  another option could be to have a CanAttack function in each attack which runs on cooldown instead if checking if within minAttackRange. Then if one attack CanAttack, that becomes the currentAttack. Otherwise if there's multiple, choose randomly between them?
    }


    public void FixedUpdate()
    {
        switch (state)
        {
            case State.cooldown:
                if (countdown <= 0 && 
                    Math.Abs(transform.position.x - player.transform.position.x) < currentAttack.currentHitbox.localScale.x + currentAttack.minAttackRange.x &&
                    Math.Abs(transform.position.y - player.transform.position.y) < currentAttack.minAttackRange.y)
                {    
                    countdown = currentAttack.windupDuration;
                    GetComponent<SpriteRenderer>().color = Color.red;
                    state = State.windup;
                    currentAttack.SetHitbox(transform, player);
                    
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
