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
	public Transform upHitbox;
	public Transform downHitbox;
    [HideInInspector] public Transform currentHitbox;

	[HideInInspector] public bool createdProgramatically = false;

	public virtual void Start() {
	}

	// todo clean up
	public virtual void SetHitbox(Transform enemy, Transform player) {
		float angle = GetAngle(enemy.position, player.transform.position);
		switch (angle) {
			case var _ when (angle > -135 && angle <= -45 && leftHitbox != null ||
							angle > -180 && angle <= 0 && upHitbox == null && downHitbox == null ||
							rightHitbox == null && upHitbox == null && downHitbox == null):
				currentHitbox = rightHitbox;
				break;
			case var _ when (angle > 45 && angle <= 135 && rightHitbox != null ||
							angle > 0 && angle <= 180 && upHitbox == null && downHitbox == null):
				currentHitbox = rightHitbox;
				break;
			case var _ when (angle > 135 && angle <= -135 && downHitbox != null ||
							angle > 90 && angle <= -90 && rightHitbox == null && leftHitbox == null):
				currentHitbox = downHitbox;
				break;
			case var _ when (angle > -45 && angle <= 45 && upHitbox != null ||
							angle > -90 && angle <= 90 && rightHitbox == null && leftHitbox == null):
				currentHitbox = upHitbox;
				break;
		}
	}

	public virtual void Windup() {}

	public virtual void StartAttacking() {
		currentHitbox.GetComponent<SpriteRenderer>().enabled = true;
        currentHitbox.GetComponent<BoxCollider2D>().enabled = true;
        currentHitbox.GetComponent<Animator>().Play(0);
	}

	public virtual void KeepAttacking() {}

	public abstract bool IsFinished();

	// todo rename finish
	public virtual void Stop() {
		currentHitbox.GetComponent<SpriteRenderer>().enabled = false;
        currentHitbox.GetComponent<BoxCollider2D>().enabled = false;
        currentHitbox.GetComponent<Animator>().StopPlayback();
	}

	public virtual void Cooldown() {}

	private float GetAngle(Vector2 pos1, Vector2 pos2) {
		return Mathf.Atan2(pos2.x - pos1.x, pos2.y - pos1.y) * Mathf.Rad2Deg;
	}
}
