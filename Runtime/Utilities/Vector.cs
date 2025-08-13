using System;
using UnityEngine;

namespace Utilities {
    public static class Vector {

        public static Vector3 GetCardinalDirection(Transform a, Transform b) {
            Vector3 direction = a.position - b.position;
            Vector3 bLocal = b.InverseTransformDirection(direction);
            Vector3 bScale = b.lossyScale;
            Vector3 directionLocal = Map((u, v) => u / v, bLocal, bScale);

            return Mathf.Abs(directionLocal.x) > Mathf.Abs(directionLocal.y) ?
                (directionLocal.x > 0 ?
                    Vector3.right : Vector3.left) :
                (directionLocal.y > 0 ?
                    Vector3.up : Vector3.down);
        }

        public static Vector3 Map(Func<float, float> f, Vector3 v) {
            return new Vector3(
                f(v.x),
                f(v.y),
                f(v.z)
            );
        }

        public static Vector3 Map(Func<float, float, float> f, Vector3 u, Vector3 v) {
            return new Vector3(
                f(u.x, v.x),
                f(u.y, v.y),
                f(u.z, v.z)
            );
        }

        public static Vector3 Map(Func<float, float, float, float> f, Vector3 u, Vector3 v, Vector3 w) {
            return new Vector3(
                f(u.x, v.x, w.x),
                f(u.y, v.y, w.y),
                f(u.z, v.z, w.z)
            );
        }

        public static float Reduce(Func<float, float, float> f, Vector3 v) {
            return f(v.x, f(v.y, v.z));
        }

        public static bool Any(Func<float, float, bool> cond, Vector3 u, Vector3 v) {
            return cond(u.x, v.x) || cond(u.y, v.y) || cond(u.z, v.z);
        }

        public static Vector3 Full(float value) {
            return Vector3.one * value;
        }

        public static bool IsValidBounds(Vector3 u, Vector3 v) {
            return !Any((a, b) => a > b, u, v);
        }

        public static Bounds GetOverlap(Bounds a, Bounds b) {
            Vector3 overlapMin = Vector3.Max(a.min, b.min);
            Vector3 overlapMax = Vector3.Min(a.max, b.max);
            return IsValidBounds(overlapMin, overlapMax) ?
                new Bounds(GetCentre(overlapMin, overlapMax), GetAbsoluteDirection(overlapMin, overlapMax)) :
                new Bounds(Vector3.zero, Vector3.zero);
        }

        public static Vector3 GetCentre(Vector3 u, Vector3 v) {
            return (u + v) / 2f;
        }

        public static Vector3 GetAbsoluteDirection(Vector3 u, Vector3 v) {
            return Map((a, b) => Mathf.Abs(a - b), u, v);
        }

        public static bool Approximately(Vector3 u, Vector3 v, float epsilon = float.Epsilon) {
            return Vector3.Distance(u, v) < epsilon;
        }
    }
}