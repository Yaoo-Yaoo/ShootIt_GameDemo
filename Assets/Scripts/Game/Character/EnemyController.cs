using System.Collections;
using SG.Tool;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SG.Game
{
    public class EnemyController : CharacterController
    {
        [SerializeField] private bool isAlive = true;
        private bool m_isAlive
        {
            get => isAlive;
            set
            {
                isAlive = value;
                if (!value)
                {
                    bodySprite.color = originColor;
                    rb.velocity = Vector3.zero;
                    rb.bodyType = RigidbodyType2D.Static;
                    GetComponent<Collider2D>().enabled = false;
                    SetDiePos();
                    anim.Play("EnemyDie");
                }
            }
        }
        
        [SerializeField] private SpriteRenderer bodySprite;
        [HideInInspector] public Transform wayPointsParent;
        private int currentTargetPointIndex = -1;
        
        private bool isHitBack = false;
        private float xVelocity;

        private bool startMoveEnemy = false;
        private float enemyStartMoveTimer = 0f;
        [SerializeField] private float enemyWaitTime = 2f;
        [SerializeField] private float hitFlashTime = 0.1f;
        private Color originColor;

        [SerializeField] private float blastSpeed = 1f;
        private bool isBlasted = false;
        private Vector2 blastedDirection = Vector2.zero;
        
        private int currentHP
        {
            get => HP;
            set
            {
                HP = value;
                if (HP <= 0)
                {
                    m_isAlive = false;
                }
            }
        }
        
        private bool m_isFacingRight = true;
        private bool isFacingRight
        {
            get => m_isFacingRight;
            set
            {
                if (m_isFacingRight != value)
                {
                    m_isFacingRight = value;
                    if (value)
                    {
                        face.localPosition = faceDefaultPos;
                    }
                    else
                    {
                        face.localPosition = new Vector3(-faceDefaultPos.x, faceDefaultPos.y, faceDefaultPos.z);
                    }
                }
            }
        }
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            
            // 记录初始值
            faceDefaultPos = face.localPosition;
        }

        private void Start()
        {
            RandomEnemy();   
            // 默认向左移动
            backOffset = 0f;
            isFacingRight = false;
            inputX = 0;
            currentTargetPointIndex = -1;
            rb.velocity = Vector3.zero;
        }

        private void Update()
        {
            if (!isAlive)
                return;
            
            if (startMoveEnemy)
            {
                FindWay();
            }
            else
            {
                enemyStartMoveTimer += Time.deltaTime;
                if (enemyStartMoveTimer >= enemyWaitTime)
                {
                    startMoveEnemy = true;
                }
            }
        }

        private void FixedUpdate()
        {
            if (!isAlive)
                return;
            
            CheckOnGround();
            if (isGrounded && currentTargetPointIndex == -1) currentTargetPointIndex = 0; 

            GetHitBack();
            Move();
            GetBlasted();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Bullet"))
            {
                xVelocity = other.GetComponent<Rigidbody2D>().velocity.x;
                isHitBack = true;
                EventManager.Instance.OnCameraShake.TriggerEvent(0.1f, 3f, 1f);
                PoolManager.Instance.ReleaseAGameObjectFromPool(other.gameObject);
                OnAttack();
                GameController.Instance.PauseGame(0.05f);
            }
        }

        private void OnAttack()
        {
            GetHurt();
            RandomBomb();
        }

        public void GetHurt()
        {
            currentHP--;
            HitFlash();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        private void FindWay()
        {
            if (currentTargetPointIndex == -1) return;

            float targetPointX = wayPointsParent.GetChild(currentTargetPointIndex).position.x;
            if (Mathf.Abs(transform.position.x - targetPointX) > 0.1f)
            {
                if (transform.position.x > targetPointX)
                {
                    inputX = -1;
                    isFacingRight = false;
                }
                else if (transform.position.x < targetPointX)
                {
                    inputX = 1;
                    isFacingRight = true;
                }
            }
            else
            {
                currentTargetPointIndex++;
                if (currentTargetPointIndex >= wayPointsParent.childCount)
                {
                    currentTargetPointIndex = 0;
                }
            }
        }

        private void HitFlash()
        {
            if (hitFlashCoroutine != null) 
                StopCoroutine(hitFlashCoroutine);

            hitFlashCoroutine = StartCoroutine(FlashBody(hitFlashTime));
        }

        private Coroutine hitFlashCoroutine = null;
        IEnumerator FlashBody(float flashTime)
        {
            float timer = 0f;
            while (timer <= 0.5f)
            {
                timer += Time.deltaTime / flashTime;
                bodySprite.color = Color.Lerp(bodySprite.color, Color.white, timer);
                yield return null;
            }
            bodySprite.color = Color.white;
            while (timer <= 1f)
            {
                timer += Time.deltaTime / flashTime;
                bodySprite.color = Color.Lerp(bodySprite.color, originColor, timer - 0.5f);
                yield return null;
            }
            bodySprite.color = originColor;
        }

        private void GetHitBack()
        {
            if (isHitBack)
            {
                if (xVelocity > 0)
                {
                    rb.AddForce(Vector2.right * backOffestSpeed * Time.deltaTime, ForceMode2D.Impulse);
                }
                else if (xVelocity < 0)
                {
                    rb.AddForce(Vector2.left * backOffestSpeed * Time.deltaTime, ForceMode2D.Impulse);
                }
                isHitBack = false;
            }
        }

        public void Blast(Vector2 direction)
        {
            isBlasted = true;
            blastedDirection = direction;
        }

        private void GetBlasted()
        {
            if (isBlasted)
            {
                rb.AddForce(blastedDirection * blastSpeed * Time.deltaTime, ForceMode2D.Impulse);
                OnAttack();
                m_isAlive = false;
                isBlasted = false;
            }
        }

        public void RandomEnemy()
        {
            bodySprite.color = Random.ColorHSV();
            originColor = bodySprite.color;
            moveSpeed = Random.Range(100, 300);
        }

        private void RandomBomb()
        {
            float randomValue = Random.Range(0.0f, 1.0f);
            if (randomValue <= 0.3f)
            {
                // 爆炸
                GameObject bombEffect = PoolManager.Instance.GetAGameObjectFromPool("BombEffect");
                bombEffect.transform.position = transform.position;
                bombEffect.GetComponent<Animator>().SetTrigger("bombTrigger");
                EventManager.Instance.OnCameraShake.TriggerEvent(0.2f, 6f, 1f);
                EventManager.Instance.OnBombBlast.TriggerEvent(transform.position, 1f);
            }
        }

        private void SetDiePos()
        {
            Vector2 edgePos = new Vector2(7, 5);
            if (transform.position.x < edgePos.x - 3)
            {
                transform.position = new Vector3(transform.position.x, -1, transform.position.z);
            }
            else
            {
                if (Mathf.Abs(transform.position.y - edgePos.y) < Mathf.Abs(transform.position.y + 1))
                {
                    transform.position = new Vector3(transform.position.x, edgePos.y, transform.position.z);
                }
                else
                {
                    transform.position = new Vector3(transform.position.x, -1, transform.position.z);
                }
            }
        }
    }
}
