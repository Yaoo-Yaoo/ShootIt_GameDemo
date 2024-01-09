using SG.Game;
using UnityEngine;

namespace SG.Tool
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform follow;
        [SerializeField] private Vector2 offset = Vector2.zero;
        [SerializeField] private float delayMultiplier = 5f;
        [SerializeField] private Vector2 softZone = Vector2.zero;

        private Vector2 defaultOffset = Vector2.zero;
        private Vector2 softZonePosX = Vector2.zero;
        private Vector2 softZonePosY = Vector2.zero;
        private Vector2 centerPos = Vector2.zero;

        private bool isShaking = false;
        private float shakeTime;
        private float shakeExistTimer = 0.0f;
        private float shakeStep;
        private float shakeSpeed;
        private Vector3 originPosition;

        public void ResetCameraXOffset(float newXOffset)
        {
            offset = new Vector2(newXOffset, offset.y);
        }

        private void Awake()
        {
            EventManager.Instance.OnCameraShake.RegisterEvent(OnCameraShake);
            
            // 记录初始值
            defaultOffset = offset;
            
            // 设置初始位置
            transform.position = new Vector3(follow.position.x + offset.x, follow.position.y + offset.y, transform.position.z);
            SetSoftZonePos();
            centerPos = transform.position;
        }
        
        private void OnDestroy()
        {
            EventManager.Instance.OnCameraShake.UnRegisterEvent(OnCameraShake);
        }

        private void SetSoftZonePos()
        {
            if (Vector2.Distance(follow.GetComponent<Rigidbody2D>().velocity, Vector2.zero) < 0.1f)
            {
                softZonePosX = new Vector2(follow.position.x - softZone.x * 0.5f, follow.position.x + softZone.x * 0.5f);
                softZonePosY = new Vector2(follow.position.y - softZone.y * 0.5f, follow.position.y + softZone.y * 0.5f);
            }
        }

        // Dead Zone 辅助
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(follow.position, new Vector3(softZone.x, softZone.y, 0));
        }

        private void LateUpdate()
        {
            if (GameController.Instance.isGameOver)
                return;
            
            if (!CheckIfInSoftZone())
                centerPos = new Vector2(follow.position.x + offset.x, follow.position.y + offset.y);

            if (Vector2.Distance(transform.position, centerPos) > 0.1f)
                transform.position = Vector3.Lerp(transform.position, new Vector3(centerPos.x, centerPos.y, transform.position.z), delayMultiplier * Time.deltaTime);
            
            // 相机震动
            if (isShaking)
            {
                shakeExistTimer += Time.fixedDeltaTime;
                if (shakeExistTimer <= shakeTime)
                {
                    // Shake
                    Vector3 RandomPoint = transform.localPosition + UnityEngine.Random.insideUnitSphere * shakeStep;
                    transform.localPosition = Vector3.Lerp(transform.localPosition, RandomPoint, Time.fixedDeltaTime * shakeSpeed);
                }
                else
                {
                    // Recover   
                    shakeExistTimer = 0f;
                    isShaking = false;
                }
            }
        }

        private bool CheckIfInSoftZone()
        {
            SetSoftZonePos();
            
            if (follow.position.x < softZonePosX.x || follow.position.x > softZonePosX.y 
             || follow.position.y < softZonePosY.x || follow.position.y > softZonePosY.y)
            {
                return false;
            }

            return true;
        }
        
        private void OnCameraShake(float shakeTime, float shakeStep, float shakeSpeed)
        {
            isShaking = true;
            this.shakeTime = shakeTime;
            this.shakeStep = shakeStep;
            this.shakeSpeed = Mathf.Clamp(shakeSpeed, 0, 1);
            shakeExistTimer = 0.0f;
        }
    }
}
