public abstract class PlayerAction {

    public abstract bool CanDo();

    public abstract bool WantsToDo();

    public bool ShouldDo() => CanDo() && WantsToDo();

    public abstract void Do();

}