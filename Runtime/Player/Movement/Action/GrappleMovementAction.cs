using UnityEngine;

public class GrappleMovementAction : PlayerMovementAction {

    public GrappleMovementAction(PlayerMovement player) : base(player) {
    }

    public override bool CanDo() => true;

    public override bool WantsToDo() {
        return Controls.isGrappling();
    }

    public override void Do() {
        player.rb.velocity = new Vector2(
            player.rb.velocity.x + (Time.deltaTime * Controls.getPlayerVelocityX() * player.grappleMoveSpeed), 
            player.rb.velocity.y);
    }

}