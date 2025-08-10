using System;
using UnityEngine;

namespace Utilities {
    class Vector {

        public static Vector3 GetCardinalDirection(Transform a, Transform b) {
            Vector3 direction = a.position - b.position;
            Vector3 bLocal = b.InverseTransformDirection(direction);
            Vector3 bScale = b.lossyScale;
            Vector3 directionLocal = Apply((u, v) => u / v, bLocal, bScale);

            return Mathf.Abs(directionLocal.x) > Mathf.Abs(directionLocal.y) ?
                (directionLocal.x > 0 ?
                    Vector3.right : Vector3.left) :
                (directionLocal.y > 0 ?
                    Vector3.up : Vector3.down);
        }

        public static Vector3 Apply(Func<float, float> f, Vector3 v) {
            return new Vector3(
                f(v.x),
                f(v.y),
                f(v.z)
            );
        }
        public static Vector3 Apply(Func<float, float, float> f, Vector3 u, Vector3 v) {
            return new Vector3(
                f(u.x, v.x),
                f(u.y, v.y),
                f(u.z, v.z)
            );
        }

        public static Vector3 Apply(Func<float, float, float, float> f, Vector3 u, Vector3 v, Vector3 w) {
            return new Vector3(
                f(u.x, v.x, w.x),
                f(u.y, v.y, w.y),
                f(u.z, v.z, w.z)
            );
        }

        public static Vector3 Full(float value) {
            return Vector3.one * value;
        }
    }
}