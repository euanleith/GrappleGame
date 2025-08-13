using UnityEngine;
using System;

using static Utilities.Layer;
using static Utilities.Vector;

public class UngrappleableRoomBoundElement : RoomBoundElement {

    private static readonly Vector3 OVERLAP_WIGGLE_ROOM = Full(0.5f); 

    new private void Reset() {
        base.Reset();
        GetComponent<SpriteRenderer>().color = Color.white;
        gameObject.layer = LayerToInt(UNGRAPPLEABLE_GROUND);
    }

    public bool temp = true;
    new private void Update() {
        base.Update();
        // todo move into if UNITY_EDITOR, i just have it here so i can read it better
        Collider2D collider = GetComponent<Collider2D>();
        RoomBound room = GetRoom();
        if (collider.enabled) {
            Collider2D[] hits = Physics2D.OverlapBoxAll(GetPosition(), GetExtent() + OVERLAP_WIGGLE_ROOM, 0f);
            foreach (Collider2D hit in hits) {
                if (IsValidCollision(hit)) {
                    if (temp) {
                        Bounds overlap = GetOverlap(collider.bounds, hit.bounds);
                        Vector3 clampDirection = RoomBoundElementEditorHelper.GetClampDirection(this);
                        Vector3 offset = clampDirection * -RoomBoundElementEditorHelper.WIDTH/2f;
                        Vector3 localPosition = overlap.center + offset - room.GetPosition();
                        float length = Reduce(Mathf.Max, overlap.size);
                        Type type = typeof(ConnectorRoomBoundElement);
                        room.AddIfAbsent(type, localPosition, length);
                        // todo and remove if overlapping with similar
                    }
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
}