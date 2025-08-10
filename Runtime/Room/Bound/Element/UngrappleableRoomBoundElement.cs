using UnityEngine;

using static Utilities.Layer;

public class UngrappleableRoomBoundElement : RoomBoundElement {

    public override Color GetColour() {
        return Color.white;
    }

    public override int GetLayer() {
        return LayerToInt(UNGRAPPLEABLE_GROUND);
    }
}