using System;
using UnityEngine;

using Utilities;

public class RoomBoundElementEditorHelper : MonoBehaviour {
    private const float WIDTH = 0.5f;
    private const float DEFAULT_LENGTH = 5f;

    private const string FOREGROUND_SORTING_LAYER = "Foreground";
    public const int FOREGROUND_LAYER = 1;
    public const int BACKGROUND_LAYER = 0;

    private const string INITIALISED_TAG = "Initialised"; // ew

    public static void CreateNewElement(Type type, RoomBound room, 
        Vector3? localPosition = null, float length = DEFAULT_LENGTH, int layer = FOREGROUND_LAYER) 
    {
        GameObject obj = new GameObject();
        InitTransform(obj.transform, room, localPosition, length, layer);
        obj.AddComponent(type);
    }

    private static void InitTransform(Transform element, RoomBound room, 
        Vector3? localPosition = null, float length = DEFAULT_LENGTH, int layer = FOREGROUND_LAYER) 
    {
        Transform elementsFolder = RoomBoundEditorHelper.GetOrCreateElementsFolder(room);
        element.parent = elementsFolder;
        element.localPosition = localPosition ?? Vector3.zero;
        element.localScale = Vector.Full(length);
        element.localRotation = Quaternion.identity;
        // todo element sprite renderer.layer = layer
        SetInitialised(element.gameObject);
    }

    private static bool IsInitialised(GameObject obj) {
        return obj.CompareTag(INITIALISED_TAG);
    }

    private static void SetInitialised(GameObject obj) {
        obj.tag = INITIALISED_TAG;
    }

    public static void OnScriptAdded(RoomBoundElement newElement) {
        RoomBound room = newElement.GetRoom();
        if (room == null) {
            Debug.LogError($"{typeof(RoomBoundElement).Name} must a child of a GameObject with a {typeof(RoomBoundElementsFolder).Name} component");
            DestroyImmediate(newElement); // using DestroyImmediate makes this class have to extend MonoBehaviour :(
            return;
        }
        // todo maybe wrap all of these in IsInitialised(), so then can create the whole element programmatically if i want
        InitGameObject(newElement);
        InitTransform(newElement, room);
        AddComponents(newElement);
        AddToRoom(room, newElement);
    }

    private static void InitGameObject(RoomBoundElement element) {
        element.gameObject.name = element.GetType().Name;
        element.gameObject.layer = element.GetLayer();
    }

    private static void InitTransform(RoomBoundElement element, RoomBound room, 
        Vector3? localPosition = null, float length = DEFAULT_LENGTH, int layer = FOREGROUND_LAYER) 
    {
        if (!IsInitialised(element.gameObject)) {
            InitTransform(element.transform, room, localPosition, length, layer);
        }
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
        sr.sprite = Resources.Load<Sprite>("Square");
        sr.color = element.GetColour();
        sr.sortingLayerName = FOREGROUND_SORTING_LAYER;
    }

    public static void Clamp(RoomBoundElement element) {
        if (element == null) return;

        RoomBound room = element.GetRoom();
        if (room == null) return;

        // must do these functions in this order
        Vector3 currentClampDirection = GetClampDirection(element);
        ClampSize(element, currentClampDirection);
        ClampPosition(element, currentClampDirection);
    }

    private static Vector3 GetClampDirection(RoomBoundElement element) {
        Vector3 elementLocalPosition = element.transform.localPosition;
        Vector3 normalisedDirection = Vector.Apply((a, b) => Mathf.Abs(a) / b, elementLocalPosition, element.GetRoom().GetExtent());
        return normalisedDirection.x > normalisedDirection.y ?
           (elementLocalPosition.x > 0 ? Vector3.right : Vector3.left) :
           (elementLocalPosition.y > 0 ? Vector3.up : Vector3.down);
    }

    // clamp room element position to (just outside) the edge of the room.
    // one axis of the element is clamped between the room bounds, and the other is snapped to the room edge
    private static void ClampPosition(RoomBoundElement element, Vector3 currentClampDirection) {
        // Vector3 roomEdgeExtent = Vector.Full(WIDTH) / 2f; // use this isntead of WIDTH if want element to be in the centre of the room bound edge
        Vector3 minBound = -element.GetRoom().GetExtent() + element.GetExtent() - Vector.Full(WIDTH);
        Vector3 maxBound = element.GetRoom().GetExtent() - element.GetExtent() + Vector.Full(WIDTH);

        Vector3 clampedPosition = Vector.Apply(Mathf.Clamp, element.transform.localPosition, minBound, maxBound);
        Vector3 edgePosition = Vector3.Scale(Vector.Apply(Mathf.Sign, element.transform.localPosition), maxBound);

        element.transform.localPosition = currentClampDirection.x != 0 ?
            new Vector2(edgePosition.x, clampedPosition.y) :
            new Vector2(clampedPosition.x, edgePosition.y);
    }

    // clamp the size of the room element to a line with the width of the room bound and length of the room element
    private static void ClampSize(RoomBoundElement element, Vector3 clampDirection) {
        element.transform.localScale = clampDirection.x != 0 ?
            new Vector2(WIDTH, element.GetLength()) :
            new Vector2(element.GetLength(), WIDTH);
    }

    private static void AddToRoom(RoomBound room, RoomBoundElement newElement) {
        if (!room.Contains(newElement)) {
            RoomBoundElement[] elements = newElement.GetComponents<RoomBoundElement>();
            if (elements.Length > 1) {
                DestroyExistingElements(elements, newElement);
            }
            newElement.GetRoom().Add(newElement);
        }
    }

    private static void DestroyExistingElements(RoomBoundElement[] elements, RoomBoundElement newElement) {
        foreach (RoomBoundElement element in elements) {
            if (element != newElement) {
                DestroyImmediate(element); // using DestroyImmediate makes this class have to extend MonoBehaviour :(
            }
        }
    }

    public static void OnDestroy(RoomBoundElement element) {
        RoomBound room = element.GetRoom();
        if (room != null) room.Remove(element);
    }
}