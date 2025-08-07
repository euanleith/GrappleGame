using UnityEngine;

namespace Utilities {
    class Vector {

        public static Vector3 GetCardinalDirection(Transform a, Transform b) {
            Vector3 direction = a.position - b.position;
            Vector3 bLocal = b.InverseTransformDirection(direction);
            Vector3 bScale = b.lossyScale;
            Vector3 directionLocal = Divide(bLocal, bScale);

            return Mathf.Abs(directionLocal.x) > Mathf.Abs(directionLocal.y) ?
                (directionLocal.x > 0 ?
                    Vector3.right : Vector3.left) :
                (directionLocal.y > 0 ?
                    Vector3.up : Vector3.down);
        }

        public static Vector3 Divide(Vector3 a, Vector3 b) {
            return new Vector3(
                a.x / b.x,
                a.y / b.y,
                a.z / b.z
            );
        }
    }
}