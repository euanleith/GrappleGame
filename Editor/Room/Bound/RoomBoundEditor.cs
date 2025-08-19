using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

[CustomEditor(typeof(RoomBound))]
public class RoomBoundEditor : Editor {
    private SerializedProperty elementsProperty;
    private Type[] elementTypes;

    void OnEnable() {
        elementsProperty = serializedObject.FindProperty("elements");
        elementTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(RoomBoundElement).IsAssignableFrom(type) && !type.IsAbstract)
            .ToArray();
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        BuildElementList();
        EditorGUILayout.Space();
        BuildAddElementButton(
            () => ShowTypeChooserMenu());

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

    private static void DeleteElement(RoomBound room, int i) {
        Undo.RecordObject(room, "Delete RoomBoundElement");
        room.RemoveElement(i);
    }

    // returns true if button is clicked
    private bool BuildAddElementButton(Action onClick) {
        return ProcessClickEvent(GUILayout.Button("Add Element"), onClick);
    }

    private void ShowTypeChooserMenu() {
        GenericMenu menu = new GenericMenu();
        foreach (Type type in elementTypes) {
            CreateTypeChooserMenuItem(menu, type.Name, 
                () => RoomBoundElementEditorHelper.CreateNewElement(type, (RoomBound)target));
        }
        menu.ShowAsContext();
    }

    private void CreateTypeChooserMenuItem(GenericMenu menu, string label, Action onClick) {
        menu.AddItem(new GUIContent(label), false, () => onClick());
    }
}
