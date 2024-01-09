using UnityEngine;
using SG.Tool;
using Random = UnityEngine.Random;

namespace SG.Game
{
    public class PlayerController : CharacterController
    {
        [Header("--Params--Jump--")] 
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private float jumpSpeed = 1f;
        [SerializeField] private float jumpGravity;
        [SerializeField] private float fallGravity;
        [SerializeField] private float jumpOnEnemyBounceSpeed = 1f;
        [Header("--Params--Fire--")]
        [SerializeField] private WeaponType weaponType = WeaponType.Weak;
        [SerializeField] private float fireFrequency = 0.2f;
        [SerializeField] private float bulletSpeed = 20f;
        [SerializeField] private Transform bulletShellParent;
        [SerializeField] private GameObject bulletShellPrefab;
        [SerializeField] private float bulletShellJumpSpeed = 1f;

        [Header("--Components--")]
        [SerializeField] private Transform gun;
        [SerializeField] private Transform bulletGeneratePoint;
        [SerializeField] private Transform bulletShellGeneratePoint;

        // 其余局部参数
        private CameraController mainCam;

        private bool isJumpButtonPressed = false;
        private float jumpCount = 1;
        
        private bool isHoldingFireButton = false;
        private float fireButtonHoldingTimer = 0.0f;

        // 初始值
        private Vector3 gunDefaultEulerAngles = Vector3.zero;
        private Vector3 bulletGenerateDefaultPos = Vector3.zero;
        
        // 血量
        private int hp
        {
            get => HP;
            set
            {
                HP = value;
                if (value <= 0)
                {
                    rb.velocity = Vector2.zero;
                    rb.bodyType = RigidbodyType2D.Static;
                    anim.Play("PlayerDie");
                    EventManager.Instance.OnGameOver.TriggerEvent();
                }
            }
        }
        
        // 玩家朝向
        private bool m_isFacingRight = false;
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
                        gun.localEulerAngles = gunDefaultEulerAngles;
                        bulletGeneratePoint.localPosition = bulletGenerateDefaultPos;
                        mainCam.ResetCameraXOffset(5);
                    }
                    else
                    {
                        face.localPosition = new Vector3(-faceDefaultPos.x, faceDefaultPos.y, faceDefaultPos.z); 
                        gun.localEulerAngles = new Vector3(gunDefaultEulerAngles.x, gunDefaultEulerAngles.y, -gunDefaultEulerAngles.z);
                        bulletGeneratePoint.localPosition = new Vector3(-bulletGenerateDefaultPos.x, bulletGenerateDefaultPos.y, bulletGenerateDefaultPos.z);
                        mainCam.ResetCameraXOffset(-5);
                    }
                }
            }
        }

        private void Awake()
        {
            mainCam = Camera.main.GetComponent<CameraController>();
            rb = GetComponent<Rigidbody2D>();

            // 记录初始值
            faceDefaultPos = face.localPosition;
            gunDefaultEulerAngles = gun.localEulerAngles;
            bulletGenerateDefaultPos = bulletGeneratePoint.localPosition;
        }

        private void Start()
        {
            // 设置默认值
            isFacingRight = true;
            fireButtonHoldingTimer = fireFrequency;
        }

        private void Update()
        {
            if (GameController.Instance.isGameOver)
                return;
            
            PlayerFireInput();
            
            if (GameController.Instance.isPaused)
                return;
            
            // Input
            PlayerMoveInput();
            PlayerJumpInput();
            PlayerChangeWeaponInput();
            
            Fire();
        }

        private void FixedUpdate()
        {
            if (GameController.Instance.isGameOver)
                return;
            
            CheckOnGround();
            
            Move();
            PlayerJump();
            
            CheckOnEnemy();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                hp--;
            }
        }

        /// <summary>
        /// 玩家输入：移动 A D
        /// </summary>
        private void PlayerMoveInput()
        {
            // Move
            inputX = Input.GetAxisRaw("Horizontal");
            if (!isHoldingFireButton)
            {
                if (inputX > 0)
                    isFacingRight = true;
                else if (inputX < 0)
                    isFacingRight = false;
            }
        }
        
        /// <summary>
        /// 玩家输入：跳跃 SPACE
        /// </summary>
        private void PlayerJumpInput()
        {
            // Jump
            if (Input.GetKeyDown(KeyCode.Space)  && jumpCount > 0)
                isJumpButtonPressed = true;
        }
        
        /// <summary>
        /// 玩家输入：射击 J
        /// </summary>
        private void PlayerFireInput()
        {
            // Shoot
            if (Input.GetKeyDown(KeyCode.J))
            {
                isHoldingFireButton = true;
            }

            if (Input.GetKeyUp(KeyCode.J))
            {
                isHoldingFireButton = false;
                fireButtonHoldingTimer = fireFrequency;
                backOffset = 0f;
            }
        }
        
        /// <summary>
        /// 玩家输入：换武器 T
        /// </summary>
        private void PlayerChangeWeaponInput()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (weaponType == WeaponType.Weak)
                {
                    weaponType = WeaponType.Strong;
                }
                else if (weaponType == WeaponType.Strong)
                {
                    weaponType = WeaponType.Weak;
                }
            }
        }

        /// <summary>
        /// 玩家跳跃
        /// </summary>
        private void PlayerJump()
        {
            // Jump
            if (isGrounded)
                jumpCount = 1;

            if (isJumpButtonPressed)
            {
                rb.AddForce(Vector2.up * jumpSpeed * Time.deltaTime, ForceMode2D.Impulse);
                jumpCount--;
                isJumpButtonPressed = false;
            }

            if (rb.velocity.y < 0)
                rb.gravityScale = fallGravity;
            else if (rb.velocity.y > 0)
                rb.gravityScale = jumpGravity;
            else
                rb.gravityScale = 1;
        }

        /// <summary>
        /// 武器射击逻辑
        /// </summary>
        private void Fire()
        {
            if (isHoldingFireButton)
            {
                fireButtonHoldingTimer += Time.deltaTime;
                if (fireButtonHoldingTimer >= fireFrequency)
                {
                    FireBullet();
                    EventManager.Instance.OnCameraShake.TriggerEvent(0.1f, 1f, 0.5f);
                    fireButtonHoldingTimer = 0.0f;
                }
            }
        }

        private void FireBullet()
        {
            if (weaponType == WeaponType.Weak)
            {
                // 单排子弹
                GenerateBullet(Random.Range(-6f, 6f));
            }
            else if (weaponType == WeaponType.Strong)
            {
                // 三排子弹
                for (int i = 0; i < 3; i++)
                {
                    GenerateBullet(-6 + i * 6);
                }
            }
        }

        private void GenerateBullet(float zEulerAngles)
        {
            GameObject bullet = PoolManager.Instance.GetAGameObjectFromPool("Bullet");
            bullet.transform.position = bulletGeneratePoint.position;
            bullet.transform.localEulerAngles = new Vector3(0, 0, zEulerAngles);
            GameObject bulletShell = Instantiate(bulletShellPrefab, bulletShellGeneratePoint.position, Quaternion.identity, bulletShellParent);
            if (isFacingRight)
            {
                bullet.transform.localScale = Vector3.one;
                bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * bulletSpeed;
                backOffset = -1f;
                bulletShell.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1,1) * bulletShellJumpSpeed, ForceMode2D.Impulse);
            }
            else
            {
                bullet.transform.localScale = new Vector3(-1, 1, 1);
                bullet.GetComponent<Rigidbody2D>().velocity = -bullet.transform.right * bulletSpeed;
                backOffset = 1f;
                bulletShell.GetComponent<Rigidbody2D>().AddForce(Vector2.one * bulletShellJumpSpeed, ForceMode2D.Impulse);
            }
            EventManager.Instance.OnShotAudioPlayed.TriggerEvent();
        }

        private void CheckOnEnemy()
        {
            Collider2D enemy = Physics2D.OverlapCircle(groundCheck.position, 0.5f, (int)enemyLayer);
            if (enemy)
            {
                // 跳起
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(Vector2.up * jumpOnEnemyBounceSpeed);
                jumpCount = 0;
                
                // 敌人受伤
                enemy.GetComponent<EnemyController>().GetHurt();
            }
        }

        public void OnPlayerDieAnimFinished()
        {
            GameController.Instance.OnGameOverPlayerAnimFinished();
        }
    }
}
