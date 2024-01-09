using System;
using UnityEngine;

namespace SG.Game
{
    public class BulletShellMove : MonoBehaviour
    {
        private int bounceTime = 0;
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Env"))
            {
                bounceTime++;
                if (bounceTime >= 2)
                {
                    Rigidbody2D rb = GetComponent<Rigidbody2D>();
                    rb.velocity = Vector2.zero;
                    rb.bodyType = RigidbodyType2D.Static;
                }
            }
        }
    }
}
