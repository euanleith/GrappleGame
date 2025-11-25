using UnityEngine;

public class JumpAction : PlayerMovementAction {

    private readonly PlayerMovementAction traversePlatformAction;

    public JumpAction(PlayerMovement player) : base(player) {
        traversePlatformAction = new TraversePlatformAction(player);
    }

    public override bool CanDo() {
        return player.isGrounded || 
            player.hitWallNormal != 0;
    }

    public override bool WantsToDo() {
        return Controls.IsJumping();
    }

    public override void Do() {
        if (traversePlatformAction.ShouldDo()) traversePlatformAction.Do();
        else Jump();
        player.isGrounded = false;
    }

    private void Jump() {
        player.UnlinkFromSwing(); // todo is this necessary?
        player.ActivateJumpCooldown();
        player.grapple.OnJump(); // todo note setting canTransformGrapple=true when doing any jump
        player.isGrounded = false;
        // todo theres a better way than if else / switch
        float velocityX = 0f;
        switch (player.hitWallNormal) {
            case > 0:
                velocityX = player.wallJumpSpeed;
                break;
            case < 0:
                velocityX = -player.wallJumpSpeed;
                break;
            case 0:
                velocityX = player.rb.velocity.x + (Controls.GetMovementX() * player.forwardAirSpeed * Time.deltaTime);
                break;
        }
        float velocityY = player.rb.velocity.y + player.jumpSpeed;
        // todo should i add this?
        //if (velocityOfGround != Vector2.zero) {
        //    velocityX = velocityOfGround.x + rb.velocity.x + (moveX * forwardAirSpeed * Time.deltaTime);
        //    velocityY = velocityOfGround.y + jumpSpeed;
        //}
        // todo might want to ignore reverse x movement after wall jump for a few seconds?
        player.rb.velocity = new Vector2(velocityX, velocityY); // note not adding rb.velocity.y 
    }

}