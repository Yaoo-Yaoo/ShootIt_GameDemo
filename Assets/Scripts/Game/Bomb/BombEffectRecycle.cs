using SG.Tool;
using UnityEngine;

namespace SG.Game
{
    public class BombEffectRecycle : MonoBehaviour
    {
        public void Recycle()
        {
            PoolManager.Instance.ReleaseAGameObjectFromPool(gameObject);
        }
    }
}
