using System.Collections;
using TMPro;
using UnityEngine;

namespace Mimic.Scripts.UI
{
    public class TimerUI : MonoBehaviour
    {
        [SerializeField] TMP_Text timerText;

        [Header("FX")]
        [SerializeField] float shakeStartTime = 1.5f;
        [SerializeField] float shakePower = 4f;
        [SerializeField] float shakeSpeed = 35f;

        float _duration;
        float _startTime;
        bool _running;
        bool _finished;
        Coroutine _hideCoroutine;

        Vector3 _basePosition;

        void Awake()
        {
            G.TimerUI = this;
            timerText.gameObject.SetActive(false);
            _basePosition = timerText.rectTransform.anchoredPosition;
        }

        void Update()
        {
            if (!_running)
                return;

            float timeLeft = Mathf.Max(0f, _duration - (Time.time - _startTime));
            SetText(timeLeft);

            if (timeLeft <= 0f && !_finished)
            {
                _finished = true;
                _running = false;

                SetText(0f);
                _hideCoroutine = StartCoroutine(HideAfterDelay());
            }
        }

        void OnDestroy()
        {
            if (G.TimerUI == this)
                G.TimerUI = null;
        }

        public void StartTimer(float duration)
        {
            _duration = duration;
            _startTime = Time.time;
            _running = true;
            _finished = false;

            if (_hideCoroutine != null)
            {
                StopCoroutine(_hideCoroutine);
                _hideCoroutine = null;
            }

            timerText.gameObject.SetActive(true);
            SetText(duration);
        }

        public void StopTimer()
        {
            _running = false;

            if (_hideCoroutine != null)
            {
                StopCoroutine(_hideCoroutine);
                _hideCoroutine = null;
            }

            timerText.transform.localScale = Vector3.one;
            timerText.rectTransform.anchoredPosition = _basePosition;
        }

        void SetText(float timeLeft)
        {
            int value = Mathf.CeilToInt(timeLeft * 100f);
            value = Mathf.Clamp(value, 0, 999);

            timerText.text = value.ToString("000");

            float pulse = 1f;
            Vector2 shakeOffset = Vector2.zero;

            if (timeLeft <= shakeStartTime && timeLeft > 0f)
            {
                float danger = Mathf.InverseLerp(shakeStartTime, 0f, timeLeft);

                pulse = 1f + Mathf.Sin(Time.time * 18f) * Mathf.Lerp(0.02f, 0.08f, danger);

                float power = Mathf.Lerp(0f, shakePower, danger);
                shakeOffset = new Vector2(
                    Mathf.Sin(Time.time * shakeSpeed) * power,
                    Mathf.Cos(Time.time * shakeSpeed * 1.37f) * power
                );
            }

            timerText.transform.localScale = Vector3.one * pulse;
            timerText.rectTransform.anchoredPosition = _basePosition + (Vector3)shakeOffset;
        }
        IEnumerator HideAfterDelay()
        {
            yield return new WaitForSeconds(0.5f);

            // вариант 1 — скрыть полностью
            timerText.gameObject.SetActive(false);

            // вариант 2 — если хочешь оставить объект:
            // timerText.text = "";

            timerText.transform.localScale = Vector3.one;
            timerText.rectTransform.anchoredPosition = _basePosition;

            _hideCoroutine = null;
        }
    }
}
