using System;
using SG.Tool;
using UnityEngine;

namespace SG.Game
{
    public class BulletBounceEffectRecycle : MonoBehaviour
    {
        private float duration;
        
        private void OnEnable()
        {
            duration = GetComponent<ParticleSystem>().main.duration;
            Invoke("Recycle", duration);
        }

        private void OnDisable()
        {
            CancelInvoke();
        }

        private void Recycle()
        {
            PoolManager.Instance.ReleaseAGameObjectFromPool(gameObject);
        }
    }
}
