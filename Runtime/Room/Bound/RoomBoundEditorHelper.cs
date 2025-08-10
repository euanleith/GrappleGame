using UnityEngine;

public static class RoomBoundEditorHelper {

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