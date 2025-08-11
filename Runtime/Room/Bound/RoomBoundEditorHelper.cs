using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RoomBoundEditorHelper : MonoBehaviour {

    private const string BOUND_ELEMENTS_FOLDER_NAME = "BoundElements";

    public static void OnScriptAdded(RoomBound room) {
        AddDefaultBoundElements(room);
    }

    private static void AddDefaultBoundElements(RoomBound room) {
        // todo make these uneditable (other than changing their type)
        //  or just put them in layer 0 and everything else in layer 1
        //  the latter would be easier but less cool

        RoomBoundElementEditorHelper.CreateNewElement(typeof(UngrappleableRoomBoundElement), room, room.GetRightBound(), room.GetSize().y, RoomBoundElementEditorHelper.BACKGROUND_LAYER);
        RoomBoundElementEditorHelper.CreateNewElement(typeof(UngrappleableRoomBoundElement), room, room.GetLeftBound(), room.GetSize().y, RoomBoundElementEditorHelper.BACKGROUND_LAYER);
        RoomBoundElementEditorHelper.CreateNewElement(typeof(UngrappleableRoomBoundElement), room, room.GetTopBound(), room.GetSize().x, RoomBoundElementEditorHelper.BACKGROUND_LAYER);
        RoomBoundElementEditorHelper.CreateNewElement(typeof(UngrappleableRoomBoundElement), room, room.GetBottomBound(), room.GetSize().x, RoomBoundElementEditorHelper.BACKGROUND_LAYER);
    }

    private static RoomBoundElementsFolder GetElementsFolder(RoomBound room) {
        return room.GetComponentInChildren<RoomBoundElementsFolder>();
    }

    public static Transform GetOrCreateElementsFolder(RoomBound room) {
        RoomBoundElementsFolder elementsFolder = GetElementsFolder(room);
        if (elementsFolder == null) {
            GameObject elementsFolderObj = new GameObject(BOUND_ELEMENTS_FOLDER_NAME);
            elementsFolderObj.transform.parent = room.transform;
            InitTransform(elementsFolderObj.transform);
            elementsFolder = elementsFolderObj.AddComponent<RoomBoundElementsFolder>();
            //EditorUtility.SetDirty(room); // todo do i need this?
        }
        return elementsFolder.transform;
    }

    public static void InitTransform(Transform transform) {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public static void DrawGizmos(RoomBound bound) {
        Gizmos.color = bound.GetColour();
        Gizmos.DrawWireCube(bound.GetPosition(), bound.GetSize());
    }

    public static void OnDestroy(RoomBound room) {
        RoomBoundElementsFolder elementsFolder = GetElementsFolder(room);
        if (elementsFolder != null) {
#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(elementsFolder.gameObject);      
#else
            DestroyImmediate(elementsFolder.gameObject); // using DestroyImmediate makes this class have to extend MonoBehaviour :(
#endif
        }
    }
}