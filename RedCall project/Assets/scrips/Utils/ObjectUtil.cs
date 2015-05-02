using UnityEngine;
using System.Collections;

namespace RedCallFrame
{
    public class ObjectUtil
    {
        public static bool ObjectIsSameOrParented(Object parent, Object child)
        {
            if (parent == child)
            {
                return true;
            }

            GameObject p = parent as GameObject;
            GameObject c = child as GameObject;
            if (p != null && c != null)
            {
                return GameObjectIsSameOrParented(p, c);
            }

            return false;
        }

        public static bool GameObjectIsSameOrParented(GameObject parent, GameObject child)
        {
            if (parent == child)
            {
                return true;
            }

            if (parent == null)
            {
                return false;
            }

            while (true)
            {
                if (child.transform.parent == null)
                {
                    break;
                }
                child = child.transform.parent.gameObject;
                if (child == null)
                {
                    break;
                }
                if (parent == child)
                {
                    return true;
                }
            }

            return false;
        }
    }


}