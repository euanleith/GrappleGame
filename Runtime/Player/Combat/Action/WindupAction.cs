using System;
using UnityEngine;

public class WindupAction : PlayerCombatAction {
    public WindupAction(PlayerCombat player) : base(player) {
    }

    public override bool CanDo() {
        return player.state == PlayerCombat.State.cooldown &&
            !player.attackDecay.IsActive() &&
            !player.player.IsStunned();
    }

    public override bool WantsToDo() {
        return Controls.IsAttacking();
    }

    public override void Do() {
        player.attackDecay.Activate(player.windupDuration);
        player.hitbox = GetHitbox();
        player.state = PlayerCombat.State.windup;
    }

    private Transform GetHitbox() {
        Vector2 inputMovement = Controls.GetMovement();
        Transform hitbox;
        if (inputMovement.x > 0 && Math.Abs(inputMovement.x) > Math.Abs(inputMovement.y)) {
            hitbox = player.rightHitbox;
        } else if (inputMovement.x < 0 && Math.Abs(inputMovement.x) > Math.Abs(inputMovement.y)) {
            hitbox = player.leftHitbox;
        } else if (inputMovement.y > 0 && Math.Abs(inputMovement.y) > Math.Abs(inputMovement.x)) {
            hitbox = player.upHitbox;
        } else if (inputMovement.y < 0 && Math.Abs(inputMovement.y) > Math.Abs(inputMovement.x)) {
            hitbox = player.downHitbox;
        } else {
            hitbox = player.rightHitbox; // todo do based on player direction - do like player.direction and deal with in player movement (most recent left/right) // or do i want nair?
        }
        return hitbox;
    }
}