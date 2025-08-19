using UnityEngine;

namespace Utilities {
    public static class GameObjectUtilities {

        public static void SetUniqueName<T>(T obj) where T : MonoBehaviour {
            SetUniqueName(obj.gameObject, obj.GetType().Name);
        }

        public static void SetUniqueName(GameObject obj, string name) {
            int count = 0;
            string uniqueName = name + count;
            while (obj.transform.parent.Find(uniqueName) != null) {
                uniqueName = name + count;
                count++;
            }
            obj.name = uniqueName;
        }
    }
}