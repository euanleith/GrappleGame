using UnityEditor;

[CustomEditor(typeof(RoomBoundElement), true)]
[CanEditMultipleObjects]
public class RoomBoundElementEditor : Editor {

    public void OnSceneGUI() {
        RoomBoundElementEditorHelper.Clamp((RoomBoundElement)target);
    }
}