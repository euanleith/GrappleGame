using System;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using Utilities;

[ExecuteAlways]
public abstract class RoomBoundElement : MonoBehaviour {

    public Color GetColour() {
        return GetComponent<SpriteRenderer>().color;
    }

    public int GetLayer() {
        return gameObject.layer;
    }

    public Vector3 GetSize() {
        return GetComponent<SpriteRenderer>().bounds.size;
    }

    public Bounds GetBounds() {
        return GetComponent<SpriteRenderer>().bounds;
    }

    public Bounds GetLocalBounds() {
        return new Bounds(GetLocalPosition(), GetSize());
    }

    public Vector3 GetExtent() {
        return GetSize() / 2;
    }

    public float GetLength() {
        return Mathf.Max(
            Mathf.Min(Mathf.Abs(GetSize().x), GetRoom().GetSize().x),
            Mathf.Min(Mathf.Abs(GetSize().y), GetRoom().GetSize().y));
    }

    public static float GetLength(Vector3 size) {
        return Mathf.Max(size.x, size.y);
    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    public Vector3 GetLocalPosition() {
        return transform.localPosition;
    }

    public RoomBound GetRoom() {
        Transform boundElementsFolder = transform.parent;
        if (boundElementsFolder.GetComponent<RoomBoundElementsFolder>() == null) return null;
        return boundElementsFolder.parent.GetComponent<RoomBound>();
    }

    public bool IsLike(Type type, Bounds bounds) {
        return GetType() == type &&
            Vector.Approximately(GetLocalPosition(), bounds.center) &&
            Mathf.Approximately(GetLength(), GetLength(bounds.size));
    }

    protected RoomBoundElement[] GetElementCollisions() {
        return Physics2D.OverlapBoxAll(GetPosition(), GetSize(), 0f)
            .Where(IsValidElementCollision)
            .Select(hit => hit.GetComponent<RoomBoundElement>())
            .ToArray();
    }

    protected bool IsValidElementCollision(Collider2D hit) {
        return enabled &&
            hit != null &&
            hit.enabled &&
            hit.GetComponent(typeof(RoomBoundElement)) != null;
    }

    protected bool InSameRoom(RoomBoundElement element) {
        return GetRoom() == element.GetRoom();
    }

    protected bool IsSameType(RoomBoundElement element) {
        return GetType() == element.GetType();
    }

    public string GetName() {
        return $"{GetRoom().name}.{name}";
    }

    protected void Reset() {
        RoomBoundElementEditorHelper.OnScriptAdded(this);
    }

    protected void Update() {
#if UNITY_EDITOR
        // ideally this would be in OnSceneGUI, but that doesn't process physics updates the same frame
        if (!Application.isPlaying) {
            if (Selection.Contains(gameObject)) {
                RoomBoundElementEditorHelper.Clamp(this);
            }
        }
#endif
    }

    private void OnDestroy() {
        RoomBoundElementEditorHelper.OnDestroy(this);
    }
}
