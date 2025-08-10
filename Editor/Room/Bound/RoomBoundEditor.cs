using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(RoomBound))]
public class RoomBoundEditor : Editor {
    private SerializedProperty elementsProperty;
    private Type[] elementTypes;

    private const string BOUND_ELEMENTS_FOLDER_NAME = "BoundElements";

    void OnEnable() {
        elementsProperty = serializedObject.FindProperty("elements");
        elementTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(RoomBoundElement).IsAssignableFrom(type) && !type.IsAbstract)
            .ToArray();
    }

    public static void DrawGizmos(RoomBound bound) {
        Gizmos.color = bound.GetColour();
        Gizmos.DrawWireCube(bound.GetPosition(), bound.GetSize());

        bool hasLoggedErr = false;
        foreach (RoomBoundElement element in bound.GetElements()) {
            if (element == null) {
                if (!bound.HasLoggedErr()) {
                    hasLoggedErr = true;
                    bound.LogError();
                }
                continue;
            }

            Gizmos.color = element.GetColour();
            Gizmos.DrawCube(element.GetPosition(), element.GetSize());
        }
        bound.SetHasLoggedErr(hasLoggedErr);
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        BuildElementList();
        EditorGUILayout.Space();
        BuildAddElementButton(
            () => ShowTypeChooserMenu());
        // todo what if element is added or deleted outside of this menu?

        serializedObject.ApplyModifiedProperties();
    }

    private void BuildElementList() {
        for (int i = 0; i < elementsProperty.arraySize; i++) {
            EditorGUILayout.BeginHorizontal();

            BuildElementField(i);
            BuildDeleteElementButton(
                () => DeleteElement((RoomBound)target, i));

            EditorGUILayout.EndHorizontal();
        }
    }

    // todo should be protected and have RoomBoundElementEditor be in child namespace Room.Bound.Element
    private void BuildElementField(int i) {
        EditorGUILayout.PropertyField(elementsProperty.GetArrayElementAtIndex(i), GUIContent.none);
    }

    private bool ProcessClickEvent(bool isClicked, Action onClick) {
        if (isClicked) onClick();
        return isClicked;
    }

    // returns true if button is clicked
    private bool BuildDeleteElementButton(Action onClick) {
        return ProcessClickEvent(GUILayout.Button("X", GUILayout.Width(20)), onClick);
    }

    // todo should be protected and have RoomBoundElementEditor be in child namespace Room.Bound.Element
    // note if this was the only element in bound elements folder, deletes bound elements folder too
    public static void DeleteElement(RoomBound room, int i) {
        List<RoomBoundElement> elements = room.GetElements();
        if (elements[i] != null) {
            Transform boundsFolder = elements[i].transform.parent;
            if (boundsFolder.childCount > 1) {
                DestroyImmediate(elements[i].gameObject);
            } else {
                DestroyImmediate(boundsFolder.gameObject);
            }
        }
        //elements.RemoveAt(i); // this is done in RoomBoundElementEditor
        EditorUtility.SetDirty(room);
    }

    // returns true if button is clicked
    private bool BuildAddElementButton(Action onClick) {
        return ProcessClickEvent(GUILayout.Button("Add Element"), onClick);
    }

    private void ShowTypeChooserMenu() {
        GenericMenu menu = new GenericMenu();
        foreach (Type type in elementTypes) {
            CreateTypeChooserMenuItem(menu, type.Name, 
                () => CreateNewElement(type, (RoomBound)target));
        }
        menu.ShowAsContext();
    }

    private void CreateTypeChooserMenuItem(GenericMenu menu, string label, Action onClick) {
        menu.AddItem(new GUIContent(label), false, () => onClick());
    }

    private void CreateNewElement(Type type, RoomBound room) {
        Transform boundsFolder = GetOrCreateBoundsFolder(room);
        CreateNewElement(type, room, boundsFolder);
        EditorUtility.SetDirty(room);
    }

    private Transform GetOrCreateBoundsFolder(RoomBound room) {
        RoomBoundElementsFolder elementsFolder = room.GetComponentInChildren<RoomBoundElementsFolder>();
        if (elementsFolder == null) {
            GameObject elementsFolderObj = new GameObject(BOUND_ELEMENTS_FOLDER_NAME);
            elementsFolderObj.transform.parent = room.transform;
            InitTransform(elementsFolderObj.transform);
            elementsFolder = elementsFolderObj.AddComponent<RoomBoundElementsFolder>();
        }
        return elementsFolder.transform;
    }

    private void CreateNewElement(Type type, RoomBound room, Transform boundsFolder) {
        GameObject newElementObj = new GameObject();
        newElementObj.transform.parent = boundsFolder;
        InitTransform(newElementObj.transform);
        RoomBoundElement newElement = newElementObj.AddComponent(type) as RoomBoundElement;
        //room.GetElements().Add(newElement); // this is done in RoomBoundElementEditor
    }

    // todo move to utils?
    private void InitTransform(Transform transform) {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
}
