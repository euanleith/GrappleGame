using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using static Utilities.Layer;

public class ConnectorRoomBoundElement : RoomBoundElement {

    new private void Reset() {
        base.Reset();
        GetComponent<SpriteRenderer>().color = Color.green;
        gameObject.layer = LayerToInt(TRANSITION); // todo idk if this is necessary, existing next rooms are all this layer though; maybe rename to ROOM_BOUND_CONNECTOR
    }

    new private void Update() {
        base.Update();
#if UNITY_EDITOR
        RoomBoundElement[] otherRoomConnectorCollisions = GetElementCollisions()
            .Where(hit => IsSameType(hit) && !InSameRoom(hit))
            .ToArray();
        if (otherRoomConnectorCollisions.Length == 0) {
            GetRoom().Remove(this);
        }
#endif
    }   
}