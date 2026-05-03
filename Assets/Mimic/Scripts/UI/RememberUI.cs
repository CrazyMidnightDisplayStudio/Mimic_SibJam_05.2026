using System.Collections;
using UnityEngine;

namespace Mimic.Scripts.UI
{
    public class RememberUI : MonoBehaviour
    {
        [SerializeField] GameObject target;

        [Header("Timing")]
        [SerializeField] float showDuration = 2f;
        [SerializeField] float blinkDuration = 2f;

        [Header("Blink")]
        [SerializeField] float blinkSpeed = 8f;

        [Header("Sound")]
        [SerializeField] AudioSource audioSource;
        [SerializeField] AudioClip appearSound;

        Coroutine _routine;
        bool _wasShowingLastFrame;

        void Awake()
        {
            if (target == null)
                target = gameObject;
        }

        void Start()
        {
            if (G.RoundController != null)
                G.RoundController.OnRoundStateChanged += HandleState;

            UpdateVisibility();
        }

        void OnDestroy()
        {
            if (G.RoundController != null)
                G.RoundController.OnRoundStateChanged -= HandleState;
        }

        void HandleState(RoundState state)
        {
            UpdateVisibility();
        }

        void UpdateVisibility()
        {
            if (G.RoundController == null)
                return;

            bool isShowing = G.RoundController.CurrentState == RoundState.Showing;

            // 👉 звук только при "входе" в Showing
            if (isShowing && !_wasShowingLastFrame)
            {
                PlaySound();
            }

            _wasShowingLastFrame = isShowing;

            // сбрасываем анимацию
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            if (isShowing)
            {
                target.SetActive(true);
                SetAlpha(1f);
                _routine = StartCoroutine(ShowSequence());
            }
            else
            {
                target.SetActive(false);
            }
        }

        IEnumerator ShowSequence()
        {
            yield return new WaitForSeconds(showDuration);

            float time = 0f;
            while (time < blinkDuration)
            {
                float t = Mathf.PingPong(Time.time * blinkSpeed, 1f);
                SetAlpha(t);

                time += Time.deltaTime;
                yield return null;
            }

            target.SetActive(false);
            _routine = null;
        }

        void PlaySound()
        {
            if (audioSource != null && appearSound != null)
            {
                audioSource.PlayOneShot(appearSound);
            }
        }

        void SetAlpha(float a)
        {
            var canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = a;
                return;
            }

            var graphic = target.GetComponent<UnityEngine.UI.Graphic>();
            if (graphic != null)
            {
                var c = graphic.color;
                c.a = a;
                graphic.color = c;
            }
        }
    }
}
