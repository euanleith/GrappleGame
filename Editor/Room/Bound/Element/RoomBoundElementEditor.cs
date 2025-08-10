using UnityEngine;
using UnityEditor;

using Utilities;

[CustomEditor(typeof(RoomBoundElement), true)]
[CanEditMultipleObjects]
public class RoomBoundElementEditor : Editor {
    private float BOUND_WIDTH = 0.5f;

    public void OnSceneGUI() {
        RoomBoundElement element = (RoomBoundElement)target;
        if (element == null) return;

        RoomBound roomBound = element.GetComponentInParent<RoomBound>();
        if (roomBound == null) return;

        Vector3 clampDirection = ClampPosition(element, roomBound);
        ClampSize(element, roomBound, clampDirection);
    }

    // clamp room element position to (just outside) the edge of the room
    private Vector3 ClampPosition(RoomBoundElement element, RoomBound room) {
        Vector3 roomCentre = room.transform.position;
        Vector3 roomExtent = room.size / 2f;
        Vector3 elementExtent = element.GetSize() / 2f;
        // Vector3 roomEdgeExtent = Vector.Full(BOUND_WIDTH) / 2f; // use this isntead of BOUND_WIDTH if want element to be in the centre of the room bound edge
        Vector3 minBound = -roomExtent + elementExtent - Vector.Full(BOUND_WIDTH);
        Vector3 maxBound = roomExtent - elementExtent + Vector.Full(BOUND_WIDTH);
        Vector3 currentPosition = element.transform.position;
        Vector3 currentOffset = currentPosition - roomCentre;
        Vector3 distance = new Vector2(
            Mathf.Abs(currentOffset.x) / roomExtent.x,
            Mathf.Abs(currentOffset.y) / roomExtent.y);
        Vector3 currentClampDirection = element.GetSize().y > element.GetSize().x ?
            (currentOffset.x > 0 ? Vector3.right : Vector3.left) :
            (currentOffset.y > 0 ? Vector3.up : Vector3.down);
        Vector3 currentDirectionBias = roomExtent;

        bool elementShouldBeHorizontal = currentClampDirection.x != 0 ?
            distance.x + currentDirectionBias.x > distance.y :
            distance.x > distance.y + currentDirectionBias.y;
        // one axis clamped between the room bounds, the other snapped to the room edge
        Vector3 clampedPosition = Vector.Apply(Mathf.Clamp, currentOffset, minBound, maxBound);
        Vector3 edgePosition = Vector3.Scale(Vector.Apply(Mathf.Sign, currentOffset), maxBound);
        Vector3 newOffset = elementShouldBeHorizontal ?
            new Vector2(edgePosition.x, clampedPosition.y) :
            new Vector2(clampedPosition.x, edgePosition.y);
        Vector3 newPosition = roomCentre + newOffset;

        if (newPosition != currentPosition) {
            // Undo.RecordObject(element.transform, "Clamp RoomBoundElement Position");
            element.transform.position = newPosition;
        }

        Vector3 newClampDirection = distance.x > distance.y ?
            (currentOffset.x > 0 ? Vector3.right : Vector3.left) :
            (currentOffset.y > 0 ? Vector3.up : Vector3.down);
        return newClampDirection;
    }

    // clamp the size of the room element to a line with the width of the room bound and length of the room element
    private void ClampSize(RoomBoundElement element, RoomBound room, Vector3 clampDirection) {
        // Undo.RecordObject(element.transform, "Clamp RoomBoundElement Size");
        float length = Mathf.Max(
            Mathf.Min(Mathf.Abs(element.GetSize().x), room.GetSize().x),
            Mathf.Min(Mathf.Abs(element.GetSize().y), room.GetSize().y));
        if (clampDirection.x != 0) {
            element.transform.localScale = new Vector2(BOUND_WIDTH, length);
        } else {
            element.transform.localScale = new Vector2(length, BOUND_WIDTH);
        }
    }
}