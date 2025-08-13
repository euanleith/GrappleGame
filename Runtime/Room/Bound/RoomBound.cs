using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// todo should be an attribute of Room rather than a monobehaviour itself
[ExecuteAlways]
public class RoomBound : MonoBehaviour {
    public Vector2 size = new Vector2(48, 27);
    [SerializeField] public List<RoomBoundElement> elements = new List<RoomBoundElement>();
    private Color colour = Color.white;

    private bool loggedErr = false;

    public Vector3 GetPosition() {
        return transform.position;
    }

    public Vector2 GetSize() {
        return size;
    }

    public Vector3 GetExtent() {
        return GetSize() / 2f;
    }

    public Vector3 GetRightBound() {
        return new Vector2(GetExtent().x, 0f);
    }

    public Vector3 GetLeftBound() {
        return new Vector2(-GetExtent().x, 0f);
    }

    public Vector3 GetTopBound() {
        return new Vector2(0f, GetExtent().y);
    }

    public Vector3 GetBottomBound() {
        return new Vector2(0f, -GetExtent().y);
    }

    public RoomBoundElement GetElement(int index) {
        return elements[index];
    }

    public void Add(RoomBoundElement element) {
        elements.Add(element);
        RoomBoundElementEditorHelper.Clamp(element);
    }

    public bool AddIfAbsent(Type type, Bounds bounds) {
        if (!elements.Any(element => element.IsLike(type, bounds))) {
            RoomBoundElementEditorHelper.CreateNewElement(
                type,
                this,
                bounds.center,
                RoomBoundElement.GetLength(bounds.size));
            return true;
        }
        return false;
    }

    public void RemoveElement(int index) {
        Remove(elements[index]);
    }

    public void Remove(RoomBoundElement element) {
        elements.Remove(element);
#if UNITY_EDITOR
        Undo.DestroyObjectImmediate(element.gameObject);
#else
        Destroy(element.gameObject);
#endif
    }

    public void RemoveWithoutDestroying(RoomBoundElement element) {
        elements.Remove(element);
    }

    public void RemoveConflictingElements(Type type, Bounds bounds) {
        for (int i = elements.Count-1; i >= 0; i--) {
            if (IsConflictingElement(elements[i], type, bounds)) {
                Remove(elements[i]);
            }
        }
    }

    public bool IsConflictingElement(RoomBoundElement element, Type type, Bounds bounds) {
        return element.GetType() == type &&
            element.GetLocalBounds().Intersects(bounds) &&
            !element.IsLike(type, bounds);
    }

    public bool Contains(RoomBoundElement element) {
        return elements.Contains(element);
    }

    // todo move to utils?
    public bool Replace(RoomBoundElement a, RoomBoundElement b) {
        int index = elements.IndexOf(a);
        if (index >= 0) {
            elements[index] = b;
            return true;
        } else {
            return false;
        }
    }

    public Color GetColour() {
        return colour;
    }
    public bool HasLoggedErr() {
        return loggedErr;
    }

    public void SetHasLoggedErr(bool loggedErr) {
        this.loggedErr = loggedErr;
    }

    public void LogError() {
        Debug.LogError("Missing RoomBoundElement " + name + " in room " + transform.parent.name);
    }

    private void Reset() {
        RoomBoundEditorHelper.OnScriptAdded(this);
    }

    private void OnDrawGizmos() {
        RoomBoundEditorHelper.DrawGizmos(this);
    }

    private void OnDestroy() {
        RoomBoundEditorHelper.OnDestroy(this);
    }
}