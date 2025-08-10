using System.Collections.Generic;
using UnityEngine;

// todo should be an attribute of Room rather than a monobehaviour itself
public class RoomBound : MonoBehaviour {
    public Vector2 size = new Vector2(48, 27);
    public List<RoomBoundElement> elements = new List<RoomBoundElement>();
    private Color colour = Color.white;

    private bool loggedErr = false;

    public Vector3 GetPosition() {
        return transform.position;
    }

    public Vector2 GetSize() {
        return size;
    }

    public List<RoomBoundElement> GetElements() {
        return elements;
    }

    // todo move to utils?
    public bool ReplaceElement(RoomBoundElement a, RoomBoundElement b) {
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

    private void OnDrawGizmos() {
        DrawGizmos(this);
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
}