using System;
using UnityEngine;

namespace SG.Game
{
    public class BulletMove : MonoBehaviour
    {
        [SerializeField] private float speed;
        
        private void Start()
        {
            GetComponent<Rigidbody2D>().velocity = transform.right * speed;
        }
    }
}
