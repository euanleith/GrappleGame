public class WallSlideAction : PlayerMovementAction {

    public WallSlideAction(PlayerMovement player) : base(player) {
    }

    public override bool CanDo() {
        return player.hitWallNormal != 0;
    }

    public override bool WantsToDo() {
        return Controls.getPlayerVelocityX() * player.hitWallNormal < 0;
    }

    public override void Do() {
        // todo
    }

}