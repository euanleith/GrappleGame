using UnityEngine;

namespace Utilities {
    public static class ColliderUtilities {

        // returns true if a is within bounds of b on x-axis
        public static bool WithinBoundsX(Collider2D a, Collider2D b, float wiggleRoom = 0f) {
            float aMin = a.bounds.min.x - wiggleRoom;
            float aMax = a.bounds.max.x + wiggleRoom;
            float bMin = b.bounds.min.x;
            float bMax = b.bounds.max.x;
            return aMax > bMin && aMin < bMax;
        }

        public static bool IsAbove(Collider2D c1, Collider2D c2) {
            return c1.bounds.min.y > c2.bounds.max.y;
        }

    }
}