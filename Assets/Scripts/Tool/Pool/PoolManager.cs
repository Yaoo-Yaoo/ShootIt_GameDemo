using System.Collections.Generic;
using UnityEngine;

namespace SG.Tool
{
    public class PoolManager 
    {
        #region Singleton
        private static PoolManager _instance;
        public static PoolManager Instance
        {
            get
            {
                if (_instance == null) _instance = new PoolManager();
                return _instance;
            }
        }
        private PoolManager() {}
        #endregion

        public Dictionary<string, Pool> allPool = new Dictionary<string, Pool>();
        public Dictionary<int, string> instanceIDToPaths = new Dictionary<int, string>();

        public GameObject GetAGameObjectFromPool(string prefabPath)
        {
            Pool pool = null;
            if (allPool.ContainsKey(prefabPath))
            {
                pool = allPool[prefabPath];
            }
            else
            {
                pool = new Pool();
                pool.prefabPath = prefabPath;
                pool.parent = new GameObject("Pool_" + prefabPath).transform;
                allPool.Add(prefabPath, pool);
            }

            GameObject obj = pool.GetObj();
            if (obj != null)
            {
                obj.SetActive(true);
            }

            return obj;
        }

        public void ReleaseAGameObjectFromPool(GameObject obj)
        {
            if (obj == null)
            {
                return;
            }

            int instanceID = obj.GetInstanceID();
            if (instanceIDToPaths.ContainsKey(instanceID))
            {
                string prefabPath = instanceIDToPaths[instanceID];
                if (allPool.ContainsKey(prefabPath))
                {
                    Pool pool = allPool[prefabPath];
                    pool.ReleaseObj(obj);
                    obj.SetActive(false);
                }
            }
        }

        public void ClearPool()
        {
            foreach (Pool pool in allPool.Values)
            {
                pool.Clear();
            }
            
            allPool.Clear();
            instanceIDToPaths.Clear();
        }
    }
}
