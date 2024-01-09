using SG.Tool;
using UnityEngine;

namespace SG.Game
{
    public class BoundaryCheck : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Bullet"))
            {
                EventManager.Instance.OnCameraShake.TriggerEvent(0.1f, 1f, 0.5f);
                PoolManager.Instance.ReleaseAGameObjectFromPool(other.gameObject);
                GameObject effect = PoolManager.Instance.GetAGameObjectFromPool("BulletBounceEffect");
                effect.transform.position = other.transform.position;
            }
        }
    }
}
