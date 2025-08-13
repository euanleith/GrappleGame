using System;
using UnityEngine;

using Utilities;

public class RoomBoundElementEditorHelper : MonoBehaviour {
    public const float WIDTH = 0.5f;
    private const float DEFAULT_LENGTH = 5f;

    private const string FOREGROUND_SORTING_LAYER = "Foreground";
    public const int FOREGROUND_LAYER = 1;
    public const int BACKGROUND_LAYER = 0;

    private const string INITIALISED_TAG = "Initialised"; // ew

    public static void CreateNewElement(Type type, RoomBound room, 
        Vector3? localPosition = null, float length = DEFAULT_LENGTH, int layer = FOREGROUND_LAYER) 
    {
        GameObject obj = new GameObject();
        BeforeScriptAdded(obj.transform, room, localPosition, length, layer);
        obj.AddComponent(type);
    }

    private static void BeforeScriptAdded(Transform element, RoomBound room,
        Vector3? localPosition = null, float length = DEFAULT_LENGTH, int layer = FOREGROUND_LAYER) 
    {
        InitTransform(element, room, localPosition, length);
        AddComponents(element, layer);
        DestroyExistingElementsInGameObject(element);

        SetInitialised(element.gameObject);
    }

    public static void OnScriptAdded(RoomBoundElement newElement) {
        RoomBound room = newElement.GetRoom();
        if (room == null) {
            Debug.LogError($"{typeof(RoomBoundElement).Name} must a child of a GameObject with a {typeof(RoomBoundElementsFolder).Name} component");
            DestroyImmediate(newElement); // using DestroyImmediate makes this class have to extend MonoBehaviour :(
            return;
        }

        if (!IsInitialised(newElement.gameObject)) {
            BeforeScriptAdded(newElement.transform, room);
        }

        AfterScriptAdded(newElement);
    }

    private static void AfterScriptAdded(RoomBoundElement element) {
        InitGameObject(element);
        InitSpriteRenderer(element);
        AddToRoom(element);
    }

    private static bool IsInitialised(GameObject obj) {
        return obj.CompareTag(INITIALISED_TAG);
    }

    private static void SetInitialised(GameObject obj) {
        obj.tag = INITIALISED_TAG;
    }

    private static void InitTransform(Transform element, RoomBound room,
        Vector3? localPosition = null, float length = DEFAULT_LENGTH) 
    {
        Transform elementsFolder = RoomBoundEditorHelper.GetOrCreateElementsFolder(room);
        element.parent = elementsFolder;
        element.localPosition = localPosition ?? Vector3.zero;
        element.localScale = Vector.Full(length);
        element.localRotation = Quaternion.identity;
    }

    private static void AddComponents(Transform element, int layer) {
        AddCollider(element);
        AddSpriteRenderer(element, layer);
    }

    private static void AddCollider(Transform element) {
        if (element.GetComponent<BoxCollider2D>() == null)
            element.gameObject.AddComponent<BoxCollider2D>();
    }

    private static void AddSpriteRenderer(Transform element, int layer) {
        if (element.GetComponent<SpriteRenderer>() == null)
            element.gameObject.AddComponent<SpriteRenderer>();

        SpriteRenderer sr = element.GetComponent<SpriteRenderer>();
        sr.material = new Material(Shader.Find("Sprites/Default"));
        sr.sprite = Resources.Load<Sprite>("Square");
        sr.sortingLayerName = FOREGROUND_SORTING_LAYER;
        sr.sortingOrder = layer;
    }

    private static void DestroyExistingElementsInGameObject(Transform newElement) {
        RoomBoundElement[] elements = newElement.GetComponents<RoomBoundElement>();
        if (elements.Length > 1) {
            foreach (RoomBoundElement element in elements) {
                DestroyImmediate(element); // using DestroyImmediate makes this class have to extend MonoBehaviour :(
            }
        }
    }

    private static void InitGameObject(RoomBoundElement element) {
        element.gameObject.name = element.GetType().Name;
        element.gameObject.layer = element.GetLayer();
    }

    private static void InitSpriteRenderer(RoomBoundElement element) {
        element.GetComponent<SpriteRenderer>().color = element.GetColour();
    }

    private static void AddToRoom(RoomBoundElement element) {
        element.GetRoom().Add(element);
    }


    public static void Clamp(RoomBoundElement element) {
        if (element == null) return;

        RoomBound room = element.GetRoom();
        if (room == null) return;

        // must do these functions in this order
        Collider2D collider = element.GetComponent<Collider2D>();
        collider.enabled = false;
        Vector3 currentClampDirection = GetClampDirection(element);
        ClampSize(element, currentClampDirection);
        ClampPosition(element, currentClampDirection);
        collider.enabled = true;
    }

    public static Vector3 GetClampDirection(RoomBoundElement element) {
        Vector3 elementLocalPosition = element.transform.localPosition;
        Vector3 normalisedDirection = Vector.Map((a, b) => Mathf.Abs(a) / b, elementLocalPosition, element.GetRoom().GetExtent());
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

        Vector3 clampedPosition = Vector.Map(Mathf.Clamp, element.transform.localPosition, minBound, maxBound);
        Vector3 edgePosition = Vector3.Scale(Vector.Map(Mathf.Sign, element.transform.localPosition), maxBound);

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

    public static void OnDestroy(RoomBoundElement element) {
        RoomBound room = element.GetRoom();
        if (room != null) room.Remove(element);
    }
}