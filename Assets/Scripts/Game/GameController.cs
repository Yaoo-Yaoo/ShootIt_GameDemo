using System.Collections;
using SG.Tool;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SG.Game
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;
        
        private bool m_IsPaused;
        public bool isPaused
        {
            get => m_IsPaused;
            private set
            {
                m_IsPaused = value;
                if (value)
                    Time.timeScale = 0;
                else
                    Time.timeScale = 1;
            }
        }
        
        public bool isGameOver { get; private set; }
        
        private void Awake()
        {
            Instance = this;
            
            EventManager.Instance.OnGameOver.RegisterEvent(OnGameOver);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isPaused = !isPaused;
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.R))
            {
                RestartGame();
            }
        }

        private void OnDestroy()
        {
            EventManager.Instance.OnGameOver.UnRegisterEvent(OnGameOver);
            StopAllCoroutines();
        }
        
        public void PauseGame(float pauseTime)
        {
            isPaused = true;
            StartCoroutine(UnPauseGame(pauseTime));
        }

        IEnumerator UnPauseGame(float waitTime)
        {
            yield return new WaitForSecondsRealtime(waitTime);
            isPaused = false;
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            isPaused = false;
            PoolManager.Instance.ClearPool();
        }
        
        private void OnGameOver()
        {
            Time.timeScale = 0.1f;
            isGameOver = true;
        }

        public void OnGameOverPlayerAnimFinished()
        {
            Time.timeScale = 1f;
        }
    }
}
