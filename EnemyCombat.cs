using System;
using UnityEngine;

public class EnemyCombat: MonoBehaviour
{
    public int damage = 1;
    public int health = 1;
    public float windupDuration = 0.5f; // todo this should be per attack
    public float cooldownDuration = 1.5f; // todo this should be per attack
    public Transform leftHitbox; // todo could get these from child
    public Transform rightHitbox;
    public float countdown;
    public enum State
    {
        windup,
        attacking,
        cooldown
    }
    public State state;
    public Vector2 minAttackRange = new Vector2(3, 2);

    Transform currentHitbox;
    Transform transform;
    Transform player;
    
    public void Start() {
        state = State.cooldown;
        currentHitbox = leftHitbox;
        transform = gameObject.transform;
        player = GetComponent<Enemy>().player;
    }

    public void FixedUpdate()
    {
        switch (state)
        {
            case State.cooldown:
                if (countdown <= 0 && 
                    Math.Abs(transform.position.x - player.transform.position.x) < currentHitbox.localScale.x + minAttackRange.x &&
                    Math.Abs(transform.position.y - player.transform.position.y) < minAttackRange.y)
                {    
                    countdown = windupDuration;
                    GetComponent<SpriteRenderer>().color = Color.red;
                    state = State.windup;
                    currentHitbox = transform.position.x > player.transform.position.x ? leftHitbox : rightHitbox;
                    
                }
                break;
            case State.attacking:
                float normalisedAnimTime = currentHitbox.GetComponent<Animator>()
                    .GetCurrentAnimatorStateInfo(0)
                    .normalizedTime; // todo would be nice to use cooldown = animationLength and iteratively subtract, but can't get animationLength :(
                if (normalisedAnimTime > 1f) // if finished attacking
                {
                    countdown = cooldownDuration;
                    currentHitbox.GetComponent<SpriteRenderer>().enabled = false;
                    currentHitbox.GetComponent<BoxCollider2D>().enabled = false;
                    currentHitbox.GetComponent<Animator>().StopPlayback();
                    state = State.cooldown;
                }
                break;
            case State.windup:
                if (countdown <= 0)
                {
                    currentHitbox.GetComponent<SpriteRenderer>().enabled = true;
                    currentHitbox.GetComponent<BoxCollider2D>().enabled = true;
                    currentHitbox.GetComponent<Animator>().Play(0);
                    GetComponent<SpriteRenderer>().color = Color.white;
                    state = State.attacking;
                }
                break;
        }   
        if (countdown > 0) {
            countdown -= Time.deltaTime;
        }
    }

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
}
