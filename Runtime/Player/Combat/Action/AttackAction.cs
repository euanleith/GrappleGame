using UnityEngine;

public class AttackAction : PlayerCombatAction {
    public AttackAction(PlayerCombat player) : base(player) {
    }

    public override bool CanDo() {
        return player.state == PlayerCombat.State.attacking;
    }

    public override bool WantsToDo() => true;

    public override void Do() {
        if (IsStillAttacking()) {
            //bool hit = HitEnemies(hitbox);
            if (player.hit && player.hitbox.position.Equals(player.downHitbox.position)) { // todo use enums?
                player.playerMovement.Jump();
            }
        } else {
            player.attackDecay.Activate(player.cooldownDuration);
            player.hitbox.GetComponent<SpriteRenderer>().enabled = false;
            player.hitbox.GetComponent<BoxCollider2D>().enabled = false;
            player.hitbox.GetComponent<Animator>().StopPlayback();
            player.hit = false;
            player.state = PlayerCombat.State.cooldown;
        }
    }

    private bool IsStillAttacking() {
        float normalisedAnimTime = player.hitbox.GetComponent<Animator>()
                    .GetCurrentAnimatorStateInfo(0)
                    .normalizedTime;
        return normalisedAnimTime <= 1f;
    }
}