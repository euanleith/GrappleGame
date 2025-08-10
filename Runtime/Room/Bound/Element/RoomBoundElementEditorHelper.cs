using UnityEngine;

public class RoomBoundElementEditorHelper : MonoBehaviour {
    // todo move to utils
    private static string FOREGROUND_SORTING_LAYER = "Foreground";

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
            } else {
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
        sr.sprite = Resources.Load<Sprite>("Square");
        sr.color = element.GetColour();
        sr.sortingLayerName = FOREGROUND_SORTING_LAYER;
        // todo set order in layer so it's above everything else
    }

    public static void OnDestroy(RoomBoundElement element) {
        RoomBound room = element.GetRoom();
        if (room != null) room.GetElements().Remove(element);
    }
}