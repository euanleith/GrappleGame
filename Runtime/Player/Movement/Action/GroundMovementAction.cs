using UnityEngine;

public class GroundMovementAction : PlayerMovementAction {

    public GroundMovementAction(PlayerMovement player) : base(player) {
    }

    public override bool CanDo() {
        return player.isGrounded && player.jumpCooldown <= 0;
    }

    public override bool WantsToDo() => true;

    public override void Do() {
        player.rb.velocity = new Vector2(Controls.getPlayerVelocityX() * player.groundSpeed, 0f);
    }

}