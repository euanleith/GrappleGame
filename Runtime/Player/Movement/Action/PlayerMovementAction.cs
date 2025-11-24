public abstract class PlayerMovementAction {

    protected PlayerMovement player;

    public PlayerMovementAction(PlayerMovement player) {
        this.player = player;
    }

    public abstract bool CanDo();

    public abstract bool WantsToDo();

    public bool ShouldDo() => CanDo() && WantsToDo();

    public abstract void Do();

}