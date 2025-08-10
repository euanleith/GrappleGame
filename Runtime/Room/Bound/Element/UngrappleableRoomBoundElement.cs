using UnityEngine;

using static Utilities.Layer;

public class UngrappleableRoomBoundElement : RoomBoundElement {

    new private void Reset() {
        base.Reset();
        GetComponent<SpriteRenderer>().color = Color.white;
        gameObject.layer = LayerToInt(UNGRAPPLEABLE_GROUND);
    }
}