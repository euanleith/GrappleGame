using UnityEngine;

public class AttackCooldownAction : PlayerCombatAction {
    public AttackCooldownAction(PlayerCombat player) : base(player) {
    }

    public override bool CanDo() {
        return player.state == PlayerCombat.State.windup &&
            !player.attackDecay.IsActive();
    }

    public override bool WantsToDo() => true;

    public override void Do() {
        player.hitbox.GetComponent<SpriteRenderer>().enabled = true;
        player.hitbox.GetComponent<BoxCollider2D>().enabled = true;
        player.hitbox.GetComponent<Animator>().Play(0);
        player.state = PlayerCombat.State.attacking;
    }

}