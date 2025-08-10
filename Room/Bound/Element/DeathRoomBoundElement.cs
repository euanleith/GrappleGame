using UnityEngine;

using static Utilities.Layer;

public class DeathRoomBoundElement : RoomBoundElement {

    public override Color GetColour() {
        return Color.red;
    }

    public override int GetLayer() {
        return LayerToInt(UNGRAPPLEABLE_DEATH); // todo player not dying on collision
    }
}