using UnityEngine;
using UnityEditor;

using Utilities;

[CustomEditor(typeof(RoomBoundElement), true)]
[CanEditMultipleObjects]
public class RoomBoundElementEditor : Editor {
    private float BOUND_WIDTH = 0.1f;

    private Vector2 clampDirection;

    public static void OnScriptAdded(RoomBoundElement newElement) {
        RoomBound room = newElement.GetRoom();
        if (room == null) {
            Debug.LogError($"{typeof(RoomBoundElement).Name} must a child of a GameObject with a {typeof(RoomBoundElementsFolder).Name} component");
            DestroyImmediate(newElement);
            return;
        }
        AddToRoom(room, newElement);
        InitGameObject(newElement);
        AddComponents(newElement);
    }

    private static void AddToRoom(RoomBound room, RoomBoundElement newElement) {
        if (!room.GetElements().Contains(newElement)) {
            RoomBoundElement[] elements = newElement.GetComponents<RoomBoundElement>();
            if (elements.Length > 1) {
                DestroyExistingElement(elements, newElement);
            }
            else {
                newElement.GetRoom().elements.Add(newElement);
            }
        }
    }

    private static void DestroyExistingElement(RoomBoundElement[] elements, RoomBoundElement newElement) {
        foreach (RoomBoundElement element in elements) {
            if (element != newElement) {
                newElement.GetRoom().ReplaceElement(element, newElement);
                DestroyImmediate(element);
            }
        }
    }

    private static void InitGameObject(RoomBoundElement element) {
        element.gameObject.name = element.GetType().Name;
        element.gameObject.layer = element.GetLayer();
    }

    private static void AddComponents(RoomBoundElement element) {
        AddCollider(element);
        AddSpriteRenderer(element);
    }

    private static void AddCollider(RoomBoundElement element) {
        if (element.GetComponent<BoxCollider2D>() == null)
            element.gameObject.AddComponent<BoxCollider2D>();
    }

    private static void AddSpriteRenderer(RoomBoundElement element) {
        if (element.GetComponent<SpriteRenderer>() == null)
            element.gameObject.AddComponent<SpriteRenderer>();

        SpriteRenderer sr = element.GetComponent<SpriteRenderer>();
        sr.material = new Material(Shader.Find("Sprites/Default"));
        sr.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/v2/Square.png");
        sr.color = element.GetColour();
        // sr.enabled is only set false in play mode
    }

    public static void OnDestroy(RoomBoundElement element) {
        RoomBound room = element.GetRoom();
        if (room != null) room.GetElements().Remove(element);
    }

    public void OnSceneGUI() {
        RoomBoundElement element = (RoomBoundElement) target;
        if (element == null) return;

        RoomBound roomBound = element.GetComponentInParent<RoomBound>();
        if (roomBound == null) return;

        ClampPosition(element, roomBound);
        ClampSize(element, roomBound);
    }

    // clamp room element position to the edge of the room
    private void ClampPosition(RoomBoundElement element, RoomBound room) {
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
        Vector3 currentDirectionBias = roomExtent;

        bool elementShouldBeHorizontal = clampDirection.x != 0 ?
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

        clampDirection = (distance.x > distance.y) ?
            (currentOffset.x > 0 ? Vector3.right : Vector3.left) :
            (currentOffset.y > 0 ? Vector3.up : Vector3.down);
    }

    // clamp the size of the room element to a line with the width of the room bound and length of the room element
    private void ClampSize(RoomBoundElement element, RoomBound room) {
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