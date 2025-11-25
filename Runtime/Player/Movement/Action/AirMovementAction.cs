using UnityEngine;

public class AirMovementAction : PlayerMovementAction {

    private readonly PlayerMovementAction wallSlideAction;

    public AirMovementAction(PlayerMovement player) : base(player) {
        wallSlideAction = new WallSlideAction(player);
    }

    public override bool CanDo() => true;

    public override bool WantsToDo() => true;

    public override void Do() {
        Vector2 inputVelocity = Controls.GetMovement();
        Vector2 airVelocity = new();
        airVelocity.x = inputVelocity.x * player.rb.velocity.x > 0 ? player.forwardAirSpeed : player.backwardAirSpeed;
        if (wallSlideAction.ShouldDo()) {
            airVelocity.y = inputVelocity.y < player.minFastFallThreshold ? player.wallFastFallSpeed : player.wallFallSpeed;
        } else {
            airVelocity.y = inputVelocity.y < player.minFastFallThreshold ? player.fastFallSpeed : player.fallSpeed;
        }
        player.rb.velocity = new Vector2(
            player.rb.velocity.x + (inputVelocity.x * airVelocity.x * Time.deltaTime), 
            player.rb.velocity.y - (airVelocity.y * Time.deltaTime));
    }

}