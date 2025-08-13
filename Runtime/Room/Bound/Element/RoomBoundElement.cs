using System;
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

    public Bounds GetLocalBounds() {
        return new Bounds(GetLocalPosition(), GetSize());
    }

    public Vector3 GetExtent() {
        return GetSize() / 2;
    }

    // todo this also clamps it to room size, should rename or something
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

    protected void Reset() {
        RoomBoundElementEditorHelper.OnScriptAdded(this);
    }

    protected void Update() {
#if UNITY_EDITOR
        // ideally this would be in OnSceneGUI, but that doesn't process physics updates the same frame
        if (Selection.Contains(gameObject)) {
            RoomBoundElementEditorHelper.Clamp(this);
        }
#endif
    }

    private void OnDestroy() {
        RoomBoundElementEditorHelper.OnDestroy(this);
    }
}
