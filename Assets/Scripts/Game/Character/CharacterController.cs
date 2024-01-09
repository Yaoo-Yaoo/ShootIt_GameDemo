using UnityEngine;

namespace SG.Game
{
    public class CharacterController : MonoBehaviour
    {
        [SerializeField] protected int HP = 0;
        [SerializeField] protected float moveSpeed = 1f;
        [SerializeField] protected float backOffestSpeed = 1f;
        [SerializeField] protected Transform face;
        [SerializeField] protected LayerMask groundLayer;
        [SerializeField] protected Transform groundCheck;
        [SerializeField] protected Animation anim;
        
        protected Rigidbody2D rb;
        protected float inputX;
        protected float backOffset;
        protected bool isGrounded = false;
        
        protected Vector3 faceDefaultPos = Vector3.zero;

        /// <summary>
        /// 移动
        /// </summary>
        protected void Move()
        {
            // Move
            rb.velocity = new Vector2 ((inputX * moveSpeed + backOffset * backOffestSpeed) * Time.deltaTime , rb.velocity.y);
        }
        
        /// <summary>
        /// 检测玩家是否落地
        /// </summary>
        protected void CheckOnGround()
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, (int)groundLayer);
        }
    }
}
