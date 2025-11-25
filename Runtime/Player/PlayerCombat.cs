using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using static Utilities.Layer;

public class PlayerCombat : MonoBehaviour {
    // todo this and EnemyCombat are basically identical, can they be generalised?

    public int damage = 1;
    public float windupDuration;
    public float cooldownDuration;
    [SerializeField] public Decay attackDecay = new Cooldown(-1);
    public Transform hitbox;
    public Transform rightHitbox;
    public Transform leftHitbox; // todo something wrong with this one, always changing while running, and animation goes on too long
    public Transform upHitbox;
    public Transform downHitbox;
    public enum State {
        windup,
        attacking,
        cooldown
    }
    public State state;
    public bool hit = false;
    public PlayerMovement playerMovement;
    public Player player;

    // todo could maybe be a map from a key to an action. in playercombat key would be state; for playermovement key would be input?
    //  and could always return a specified type. in playercombat this would be the next state; for playermovement this would be the velocity to set rb to
    //  and could have player extend from Actor, which implements actions in Update. and then could access the contents of player from within each action without having to have each action have a Player attribute?
    private List<PlayerCombatAction> actions; // only first which satisfies action.ShouldDo() will be performed

    private void Start() {
        state = State.cooldown;
        DisableAttackHitboxes();
        playerMovement = GetComponent<PlayerMovement>();
        player = GetComponent<Player>();
        actions = new List<PlayerCombatAction> {
            new WindupAction(this),
            new AttackAction(this),
            new AttackCooldownAction(this)
        };
    }

    private void DisableAttackHitboxes() {
        rightHitbox.GetComponent<BoxCollider2D>().enabled = false;
        leftHitbox.GetComponent<BoxCollider2D>().enabled = false;
        upHitbox.GetComponent<BoxCollider2D>().enabled = false;
        downHitbox.GetComponent<BoxCollider2D>().enabled = false;
    }

    private void Update() {
        actions.Where(action => action.ShouldDo()).FirstOrDefault()?.Do(); 
        attackDecay.Update();
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (hit) return; // only hit one object per attack
        if (IsEnemyLayer(collider.gameObject.layer)) {
            hit = true;
            if (collider.gameObject.GetComponent<EnemyLayer>()) {
                collider.gameObject.GetComponent<EnemyLayer>().OnHit(damage);
            } else if (!LayerEquals(collider.gameObject.layer, ENEMY_INVULNERABLE)) {
                collider.gameObject.GetComponentInParent<Enemy>().OnHit(damage);
            }
        }
    }
}