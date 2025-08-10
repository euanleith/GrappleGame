using UnityEngine;

[ExecuteAlways]
public abstract class RoomBoundElement : MonoBehaviour {
    public Vector3 GetSize() {
        return GetComponent<SpriteRenderer>().bounds.size;
    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    public abstract Color GetColour();

    public abstract int GetLayer();

    private void Reset() {
        RoomBoundElementEditor.OnScriptAdded(this);
    }

    private void OnDestroy() {
        RoomBoundElementEditor.OnDestroy(this);
    }

    public RoomBound GetRoom() {
        Transform boundElementsFolder = transform.parent;
        if (boundElementsFolder.GetComponent<RoomBoundElementsFolder>() == null) return null;
        return boundElementsFolder.parent.GetComponent<RoomBound>();
    }
}
