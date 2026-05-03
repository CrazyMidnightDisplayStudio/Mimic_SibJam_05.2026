using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mimic.Scripts.UI.Leaderboard
{
    public class LeaderboardRowUI : MonoBehaviour
    {
        [SerializeField] TMP_Text placeText;
        [SerializeField] TMP_Text nameText;
        [SerializeField] TMP_Text scoreText;
        [SerializeField] Image background;
        [SerializeField] CanvasGroup canvasGroup;

        [Header("Colors")]
        [SerializeField] Color normalColor = Color.white;
        [SerializeField] Color currentPlayerColor = new Color(1f, 0.82f, 0.2f, 1f);

        RectTransform _rect;

        void Awake()
        {
            _rect = GetComponent<RectTransform>();

            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        public void SetData(int place, string playerName, int score, bool isCurrentPlayer)
        {
            placeText.text = place.ToString();
            nameText.text = playerName;
            scoreText.text = score.ToString();

            if (background != null)
                background.color = isCurrentPlayer ? currentPlayerColor : normalColor;
        }

        public void SetPosition(Vector2 position)
        {
            _rect.anchoredPosition = position;
        }

        public IEnumerator AnimateFromBottom(Vector2 targetPosition, float fromOffsetY, float duration)
        {
            canvasGroup.alpha = 0f;

            Vector2 startPosition = targetPosition + Vector2.down * fromOffsetY;
            _rect.anchoredPosition = startPosition;

            float time = 0f;

            while (time < duration)
            {
                float t = time / duration;
                t = EaseOutBack(t);

                _rect.anchoredPosition = Vector2.LerpUnclamped(startPosition, targetPosition, t);
                canvasGroup.alpha = Mathf.Clamp01(time / (duration * 0.5f));

                time += Time.deltaTime;
                yield return null;
            }

            _rect.anchoredPosition = targetPosition;
            canvasGroup.alpha = 1f;
        }

        public IEnumerator MoveTo(Vector2 targetPosition, float duration)
        {
            Vector2 startPosition = _rect.anchoredPosition;
            float time = 0f;

            while (time < duration)
            {
                float t = time / duration;
                t = EaseOutCubic(t);

                _rect.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);

                time += Time.deltaTime;
                yield return null;
            }

            _rect.anchoredPosition = targetPosition;
        }
        public IEnumerator AnimatePlaceAndScore(
            int fromPlace,
            int toPlace,
            int fromScore,
            int toScore,
            float duration)
        {
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;

                float t = Mathf.Clamp01(time / duration);
                t = 1f - Mathf.Pow(1f - t, 3f);

                int place = Mathf.RoundToInt(Mathf.Lerp(fromPlace, toPlace, t));
                int score = Mathf.RoundToInt(Mathf.Lerp(fromScore, toScore, t));

                placeText.text = place.ToString();
                scoreText.text = score.ToString();

                yield return null;
            }

            placeText.text = toPlace.ToString();
            scoreText.text = toScore.ToString();
        }

        static float EaseOutCubic(float t)
        {
            return 1f - Mathf.Pow(1f - t, 3f);
        }

        static float EaseOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;

            return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
        }
    }
}
