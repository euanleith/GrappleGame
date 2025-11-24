using UnityEngine;

public class WallClimbAction : PlayerMovementAction {

    public WallClimbAction(PlayerMovement player) : base(player) {
    }

    public override bool CanDo() {
        return player.isGrounded &&
            player.hitWallNormal != 0;
    }

    public override bool WantsToDo() {
        return player.rb.velocity.x * player.hitWallNormal < 0;
    }

    public override void Do() {
        player.rb.velocity = new Vector2(0, player.wallClimbSpeed);
    }

}