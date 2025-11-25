using System.Threading.Tasks;
using UnityEngine;

using static Utilities.Layer;

public class TraversePlatformAction : PlayerMovementAction {

    private static readonly int TRAVERSE_COOLDOWN = 500; // ms

    public TraversePlatformAction(PlayerMovement player) : base(player) {
    }

    public override bool CanDo() {
        return player.isGrounded &&
            player.transform.parent.GetComponent<PlatformEffector2D>() != null;
    }

    public override bool WantsToDo() {
        return Controls.GetMovementY() < 0;
    }

    public override void Do() {
        DoAsync(player.transform.parent.GetComponent<PlatformEffector2D>());
    }

    private async void DoAsync(PlatformEffector2D platform) {
        platform.colliderMask = RemoveFromLayerMask(platform.colliderMask, PLAYER);
        await Task.Delay(TRAVERSE_COOLDOWN);
        platform.colliderMask = AddToLayerMask(platform.colliderMask, PLAYER);
    }

}