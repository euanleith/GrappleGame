using UnityEngine;

using static Utilities.Layer;

public class DeathRoomBoundElement : RoomBoundElement {

    new private void Reset() {
        base.Reset();
        GetComponent<SpriteRenderer>().color = Color.red;
        gameObject.layer = LayerToInt(UNGRAPPLEABLE_DEATH);
    }
}