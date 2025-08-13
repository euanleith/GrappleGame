using UnityEngine;
using System;

using Utilities;
using static Utilities.Layer;

public class UngrappleableRoomBoundElement : RoomBoundElement {

    private static readonly Vector3 OVERLAP_WIGGLE_ROOM = Vector.Full(0.5f); 

    new private void Reset() {
        base.Reset();
        GetComponent<SpriteRenderer>().color = Color.white;
        gameObject.layer = LayerToInt(UNGRAPPLEABLE_GROUND);
    }

    new private void Update() {
        base.Update();
        // todo move into if UNITY_EDITOR, i just have it here so i can read it better
        Collider2D collider = GetComponent<Collider2D>();
        RoomBound room = GetRoom();
        if (collider.enabled) {
            Collider2D[] hits = Physics2D.OverlapBoxAll(GetPosition(), GetExtent() + OVERLAP_WIGGLE_ROOM, 0f);
            foreach (Collider2D hit in hits) {
                if (IsValidCollision(hit)) {
                    Bounds bounds = GetOverlap(hit);
                    Type type = typeof(ConnectorRoomBoundElement);

                    room.AddIfAbsent(type, bounds);
                    room.RemoveConflictingElements(type, bounds);
                }
            }
        }
    }

    private bool IsValidCollision(Collider2D hit) {
        return hit.GetComponent<UngrappleableRoomBoundElement>() != null &&
            !AreInTheSameRoom(hit.transform, transform) &&
            hit.enabled;
    }

    private bool AreInTheSameRoom(Transform a, Transform b) {
        return a.parent.parent == b.parent.parent; // todo i guess checking all parents for Room component would be better, but would be slower too
    }

    private Bounds GetOverlap(Collider2D hit) {
        Bounds overlap = Vector.GetOverlap(GetComponent<Collider2D>().bounds, hit.bounds);
        // todo could i just make a Clamp() function which uses bounds instead of element
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