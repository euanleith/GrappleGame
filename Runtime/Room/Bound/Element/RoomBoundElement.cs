using UnityEngine;

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

    public Vector3 GetPosition() {
        return transform.position;
    }

    public RoomBound GetRoom() {
        Transform boundElementsFolder = transform.parent;
        if (boundElementsFolder.GetComponent<RoomBoundElementsFolder>() == null) return null;
        return boundElementsFolder.parent.GetComponent<RoomBound>();
    }

    protected void Reset() {
        RoomBoundElementEditorHelper.OnScriptAdded(this);
    }

    private void OnDestroy() {
        RoomBoundElementEditorHelper.OnDestroy(this);
    }
}
