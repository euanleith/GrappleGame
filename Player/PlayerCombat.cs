using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCombat : MonoBehaviour
{
    // todo this and EnemyCombat are basically identical, can they be generalised?

    public int damage = 1;
    private Animator anim;
    public float windupDuration;
    public float cooldownDuration;
    public float countdown;
    public Transform hitbox;
    public Transform rightHitbox;
    public Transform leftHitbox; // todo something wrong with this one, always changing while running, and animation goes on too long
    public Transform upHitbox;
    public Transform downHitbox;
    public LayerMask enemyLayer;
    public enum State
    {
        windup,
        attacking,
        cooldown
    }
    public State state;
    public bool hit = false;

    private void Start()
    {
        state = State.cooldown;
        rightHitbox.GetComponent<BoxCollider2D>().enabled = false;
        leftHitbox.GetComponent<BoxCollider2D>().enabled = false;
        upHitbox.GetComponent<BoxCollider2D>().enabled = false;
        downHitbox.GetComponent<BoxCollider2D>().enabled = false;
    }

    void Update()
    {
        switch (state)
        {
            case State.cooldown:
                if (Input.GetButton("Fire3") && countdown <= 0)
                {
                    countdown = windupDuration;
                    hitbox = GetHitbox();
                    state = State.windup;
                }
                break;
            case State.attacking:
                float normalisedAnimTime = hitbox.GetComponent<Animator>()
                    .GetCurrentAnimatorStateInfo(0)
                    .normalizedTime;  // todo would be nice to use cooldown = animationLength and iteratively subtract, but can't get animationLength :(
                if (normalisedAnimTime <= 1f) // if still attacking
                {
                    //bool hit = HitEnemies(hitbox);
                    if (hit && hitbox.position.Equals(downHitbox.position)) // todo use enums?
                    {
                        GetComponent<PlayerControls>().Jump();
                    }
                }
                else 
                {
                    countdown = cooldownDuration;
                    hitbox.GetComponent<SpriteRenderer>().enabled = false;
                    hitbox.GetComponent<BoxCollider2D>().enabled = false;
                    hitbox.GetComponent<Animator>().StopPlayback();
                    hit = false;
                    state = State.cooldown;
                }
                break;
            case State.windup:
                if (countdown <= 0)
                {
                    hitbox.GetComponent<SpriteRenderer>().enabled = true;
                    hitbox.GetComponent<BoxCollider2D>().enabled = true;
                    hitbox.GetComponent<Animator>().Play(0);
                    state = State.attacking;
                }
                break;
        }
        if (countdown > 0) {
            countdown -= Time.deltaTime;
        }
    }

    private Transform GetHitbox() 
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        Transform hitbox;
        if (moveX > 0 && Math.Abs(moveX) > Math.Abs(moveY))
        {
            hitbox = rightHitbox;
        }
        else if (moveX < 0 && Math.Abs(moveX) > Math.Abs(moveY)) 
        {
            hitbox = leftHitbox;
        }
        else if (moveY > 0 && Math.Abs(moveY) > Math.Abs(moveX)) 
        {
            hitbox = upHitbox;
        } 
        else if (moveY < 0 && Math.Abs(moveY) > Math.Abs(moveX))
        {
            hitbox = downHitbox;
        }
        else
        {
            hitbox = rightHitbox; // todo do based on player direction - do like player.direction and deal with in player movement (most recent left/right) // or do i want nair?
        }
        return hitbox;
    }

    // todo something should happen to player after hitting enemy
    public void OnTriggerEnter2D(Collider2D collider) {
        if (hit) return; // only hit one object per attack
        switch (collider.gameObject.layer) {
            case 8: // Enemy
            case 19: // EnemyTraversable
                if (!collider.gameObject.GetComponent<EnemyLayer>()) {
                    collider.gameObject.GetComponentInParent<Enemy>().OnHit(damage);
                } else {
                    collider.gameObject.GetComponent<EnemyLayer>().OnHit(damage);
                }
                hit = true;
                break;
            case 17: // EnemyInvulnerable
                hit = true;
                break;
            }
    }
}