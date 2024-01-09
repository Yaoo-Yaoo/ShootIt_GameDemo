using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SG.Game
{
    public class EnemyGenerator : MonoBehaviour
    {
        [Header("--Params--")]
        [SerializeField] private int count = 10;
        
        [Header("--Components--")]
        [SerializeField] private Transform generateArea;
        [SerializeField] private Transform wayPointsParent;
        [SerializeField] private Transform enemiesParent;
        
        [Header("--References--")]
        [SerializeField] private GameObject enemyPrefab;

        private GameObject[] allEnemies;

        private void Awake()
        {
            EventManager.Instance.OnBombBlast.RegisterEvent(OnBombBlast);
            
            GenerateEnemies();
        }
        
        private void OnDestroy()
        {
            EventManager.Instance.OnBombBlast.UnRegisterEvent(OnBombBlast);
        }

        private void GenerateEnemies()
        {
            allEnemies = new GameObject[count];
            for (int i = 0; i < count; i++)
            {
                GameObject enemy = Instantiate(enemyPrefab, RandomGeneratingPos(), Quaternion.identity, enemiesParent);
                enemy.GetComponent<EnemyController>().wayPointsParent = wayPointsParent;
                allEnemies[i] = enemy;
            }
        }

        private Vector3 RandomGeneratingPos()
        {
            Vector3 leftPos = generateArea.GetChild(0).position;
            Vector3 rightPos = generateArea.GetChild(1).position;
            return new Vector3(Random.Range(leftPos.x, rightPos.x), leftPos.y, leftPos.z);
        }
        
        private void OnBombBlast(Vector3 centerPos, float radius)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject enemy = allEnemies[i];
                if (Vector3.Distance(enemy.transform.position, centerPos) <= radius)
                {
                    // 被击飞
                    enemy.GetComponent<EnemyController>().Blast(new Vector2((enemy.transform.position - centerPos).x, 1));
                }
            }
        }
    }
}
