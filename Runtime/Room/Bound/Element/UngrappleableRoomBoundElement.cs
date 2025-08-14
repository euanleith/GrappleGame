using UnityEngine;
using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

using Utilities;
using static Utilities.Layer;

public class UngrappleableRoomBoundElement : RoomBoundElement {

    new private void Reset() {
        base.Reset();
        GetComponent<SpriteRenderer>().color = Color.white;
        gameObject.layer = LayerToInt(UNGRAPPLEABLE_GROUND);
    }

    new private void Update() {
        base.Update();
#if UNITY_EDITOR
        RoomBoundElement[] otherRoomUngrappleableElementCollisions = GetElementCollisions()
            .Where(hit => IsSameType(hit) && !InSameRoom(hit))
            .ToArray();
        for (int i = otherRoomUngrappleableElementCollisions.Length-1; i >= 0; i--) {
            Bounds bounds = GetOverlap(otherRoomUngrappleableElementCollisions[i].GetBounds());
            Type type = typeof(ConnectorRoomBoundElement);

            GetRoom().AddIfAbsent(type, bounds);
            GetRoom().RemoveConflictingElements(type, bounds);
        }
#endif
    }

    // todo should just be able to use Vector.GetOverlap(GetBounds(), bounds + offset)
    private Bounds GetOverlap(Bounds bounds) {
        Bounds overlap = Vector.GetOverlap(GetBounds(), bounds);
        Vector3 clampDirection = RoomBoundElementEditorHelper.GetClampDirection(this);
        Vector3 offset = clampDirection * -RoomBoundElementEditorHelper.WIDTH / 2f;
        Vector3 localPosition = overlap.center + offset - GetRoom().GetPosition();
        float length = Vector.Reduce(Mathf.Max, overlap.size);
        Vector3 size = clampDirection.x != 0 ?
            new Vector2(length, RoomBoundElementEditorHelper.WIDTH) :
            new Vector2(RoomBoundElementEditorHelper.WIDTH, length);
        return new Bounds(localPosition, size);
    }
}