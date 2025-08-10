using System;
using UnityEngine;

[ExecuteAlways]
public class RoomBoundElementsFolder : MonoBehaviour {

    private void Reset() {
        bool success = EnforceRules();
        if (!success) {
            DestroyImmediate(this);
            return;
        }
    }

    private bool EnforceRules() {
        Func<bool>[] rules = new Func<bool>[] {
            EnforceHasValidParent,
            EnforceIsSingleton
        };
        
        foreach (Func<bool> rule in rules) {
            bool success = rule.Invoke();
            if (!success) return false;
        }
        return true;
    }

    private bool EnforceHasValidParent() {
        RoomBound room = GetComponentInParent<RoomBound>();
        if (room == null) {
            Debug.LogError($"{GetType().Name} must be a child of a GameObject with a {typeof(RoomBound).Name} component");
            return false;
        }
        return true;
    }

    private bool EnforceIsSingleton() {
        RoomBound room = GetComponentInParent<RoomBound>();
        RoomBoundElementsFolder[] siblings = room.GetComponentsInChildren<RoomBoundElementsFolder>();
        if (siblings.Length > 1) {
            Debug.LogError($"{typeof(RoomBound).Name} '{room.name}' already has a {GetType().Name}");
            return false;
        }
        return true;
    }
}