using System.Collections;
using UnityEngine;

namespace SG.Game
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private RectTransform gameoverText;
        [SerializeField] private float delayTime = 2f;
        [SerializeField] private float showDuration = 1f;
        [SerializeField] private Vector2 posYRange;

        private void Awake()
        {
            EventManager.Instance.OnGameOver.RegisterEvent(OnGameOver);
        }

        private void OnDestroy()
        {
            EventManager.Instance.OnGameOver.UnRegisterEvent(OnGameOver);
            StopAllCoroutines();
        }

        private void OnGameOver()
        {
            if (moveGameOverTextCoroutine != null) StopCoroutine(moveGameOverTextCoroutine);
            moveGameOverTextCoroutine = StartCoroutine(MoveGameOverText(delayTime, showDuration));
        }

        private Coroutine moveGameOverTextCoroutine = null;
        IEnumerator MoveGameOverText(float delayTime, float duration)
        {
            yield return new WaitForSecondsRealtime(delayTime);
            
            float timer = 0.0f;
            while (timer <= 1f)
            {
                timer += Time.unscaledDeltaTime / duration;
                gameoverText.anchoredPosition = new Vector2(gameoverText.anchoredPosition.x, posYRange.x) + new Vector2(0, posYRange.y - posYRange.x) * timer;
                yield return null;
            }

            gameoverText.anchoredPosition = new Vector2(gameoverText.anchoredPosition.x, posYRange.y);
        }
    }
}
