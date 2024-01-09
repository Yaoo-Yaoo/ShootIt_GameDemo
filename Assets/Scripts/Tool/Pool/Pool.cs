using System.Collections.Generic;
using UnityEngine;

namespace SG.Tool
{
    public class Pool
    {
        public string prefabPath;
        public Transform parent;

        private List<GameObject> usingList = new List<GameObject>();
        private List<GameObject> freeList = new List<GameObject>();

        public GameObject GetObj()
        {
            GameObject obj = null;
            if (freeList.Count > 0)
            {
                obj = freeList[0];
                usingList.Add(obj);
                freeList.RemoveAt(0);
            }
            else
            {
                GameObject prefab = Resources.Load<GameObject>(prefabPath);
                obj = GameObject.Instantiate(prefab, parent);
                int id = obj.GetInstanceID();
                PoolManager.Instance.instanceIDToPaths.Add(id, prefabPath);
                usingList.Add(obj);
            }

            return obj;
        }

        public void ReleaseObj(GameObject obj)
        {
            usingList.Remove(obj);
            freeList.Add(obj);
        }

        public void Clear()
        {
            foreach (GameObject item in usingList)
            {
                GameObject.Destroy(item);
            }
            foreach (GameObject item in freeList)
            {
                GameObject.Destroy(item);
            }
            usingList.Clear();
            freeList.Clear();
            GameObject.Destroy(parent.gameObject);
        }
    }
}
