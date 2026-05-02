using System.Collections;
using TMPro;
using UnityEngine;

namespace Mimic.Scripts.UI
{
    public class ScoreResultUI : MonoBehaviour
    {
        [SerializeField] TMP_Text scoreText;

        [Header("Animation")]
        [SerializeField] float rollDuration = 2.0f;
        [SerializeField] AnimationCurve rollCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Audio")]
        [SerializeField] AudioSource audioSource;
        [SerializeField] AudioClip tickClip;
        [SerializeField] float tickStep = 1f;
        [SerializeField] float minPitch = 0.8f;
        [SerializeField] float maxPitch = 1.6f;

        Coroutine _rollCoroutine;
        Coroutine _punchCoroutine;

        float _shownTotalScore;
        float _nextTickScore;

        void Awake()
        {
            G.ScoreResultUI = this;
            scoreText.gameObject.SetActive(false);
        }

        void Start()
        {
            G.RoundController.OnRoundStateChanged += HandleState;
            G.GameManager.OnScoreChanged += HandleRoundScoreChanged;
        }

        void OnDestroy()
        {
            if (G.RoundController != null)
                G.RoundController.OnRoundStateChanged -= HandleState;

            if (G.GameManager != null)
                G.GameManager.OnScoreChanged -= HandleRoundScoreChanged;

            if (G.ScoreResultUI == this)
                G.ScoreResultUI = null;
        }

        void HandleState(RoundState state)
        {
            bool isResult = state == RoundState.Calculating;
            scoreText.gameObject.SetActive(isResult);

            if (!isResult)
            {
                StopAnimations();
                scoreText.transform.localScale = Vector3.one;
            }
        }

        void HandleRoundScoreChanged(float roundScore)
        {
            ShowScore(roundScore, G.GameManager.TotalScore);
        }

        public void ShowScore(float roundScore, float totalScore)
        {
            scoreText.gameObject.SetActive(true);

            StopAnimations();

            scoreText.transform.localScale = Vector3.one;
            _rollCoroutine = StartCoroutine(RollScore(roundScore, _shownTotalScore, totalScore));
        }

        void StopAnimations()
        {
            if (_rollCoroutine != null)
            {
                StopCoroutine(_rollCoroutine);
                _rollCoroutine = null;
            }

            if (_punchCoroutine != null)
            {
                StopCoroutine(_punchCoroutine);
                _punchCoroutine = null;
            }
        }

        IEnumerator RollScore(float roundScore, float fromTotalScore, float toTotalScore)
        {
            float time = 0f;
            _nextTickScore = tickStep;

            while (time < rollDuration)
            {
                time += Time.deltaTime;

                float t = Mathf.Clamp01(time / rollDuration);
                float curvedT = rollCurve.Evaluate(t);

                float currentRoundScore = Mathf.Lerp(0f, roundScore, curvedT);
                float currentTotalScore = Mathf.Lerp(fromTotalScore, toTotalScore, curvedT);

                TryPlayTick(currentRoundScore, roundScore);
                SetText(currentRoundScore, currentTotalScore);

                yield return null;
            }

            SetText(roundScore, toTotalScore);
            _shownTotalScore = toTotalScore;

            yield return new WaitForSeconds(0.03f);
            PlayFinalTick();

            _rollCoroutine = null;
            _punchCoroutine = StartCoroutine(PunchScale());
        }

        void SetText(float roundScore, float totalScore)
        {
            scoreText.text =
                $"Round: {roundScore:0.00}\n" +
                $"Total: {totalScore:0.00}";
        }

        void TryPlayTick(float currentScore, float targetScore)
        {
            if (audioSource == null || tickClip == null)
                return;

            if (tickStep <= 0f)
                return;

            if (currentScore < _nextTickScore)
                return;

            float normalizedScore = Mathf.InverseLerp(0f, Mathf.Max(1f, targetScore), currentScore);

            audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, normalizedScore);
            audioSource.PlayOneShot(tickClip);

            _nextTickScore += tickStep;
        }

        void PlayFinalTick()
        {
            if (audioSource == null || tickClip == null)
                return;

            audioSource.Stop();
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(tickClip);
        }

        IEnumerator PunchScale()
        {
            Vector3 baseScale = scoreText.transform.localScale;
            Vector3 bigScale = baseScale * 1.15f;

            float duration = 0.15f;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / duration);
                scoreText.transform.localScale = Vector3.Lerp(baseScale, bigScale, t);
                yield return null;
            }

            time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / duration);
                scoreText.transform.localScale = Vector3.Lerp(bigScale, baseScale, t);
                yield return null;
            }

            scoreText.transform.localScale = baseScale;
            _punchCoroutine = null;
        }
    }
}
