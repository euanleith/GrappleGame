using UnityEngine;

using static Utilities.Layer;

public class ConnectorRoomBoundElement : RoomBoundElement {

    new private void Reset() {
        base.Reset();
        GetComponent<SpriteRenderer>().color = Color.green;
        gameObject.layer = LayerToInt(TRANSITION); // todo idk if this is necessary, existing next rooms are all this layer though; maybe rename to ROOM_BOUND_CONNECTOR
    }
}