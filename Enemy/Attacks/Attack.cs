using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : MonoBehaviour
{
	public int damage = 1;
	public float windupDuration = 0.5f;
    public float cooldownDuration = 1.5f;
	public Vector2 minAttackRange = new Vector2(3, 2);
	public Transform leftHitbox; // todo could get these from child
    public Transform rightHitbox;
    [HideInInspector] public Transform currentHitbox;

	[HideInInspector] public bool createdProgramatically = false;

	public virtual void Start() {
		currentHitbox = leftHitbox;
	}

	public virtual void SetHitbox(Transform enemy, Transform player) {
        currentHitbox = enemy.position.x > player.transform.position.x ? leftHitbox : rightHitbox; // this should probably be in each attack, since some might have down hitboxes etc
	}

	public virtual void StartAttacking() {
		currentHitbox.GetComponent<SpriteRenderer>().enabled = true;
        currentHitbox.GetComponent<BoxCollider2D>().enabled = true;
        currentHitbox.GetComponent<Animator>().Play(0);
	}

	public abstract void KeepAttacking();

	public abstract bool IsFinished();

	public virtual void Stop() {
		currentHitbox.GetComponent<SpriteRenderer>().enabled = false;
        currentHitbox.GetComponent<BoxCollider2D>().enabled = false;
        currentHitbox.GetComponent<Animator>().StopPlayback();
	}
}
