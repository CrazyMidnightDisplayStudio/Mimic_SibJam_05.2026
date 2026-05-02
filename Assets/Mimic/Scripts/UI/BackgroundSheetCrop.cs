using System.Collections;
using UnityEngine;

namespace Mimic.Scripts.UI
{
    public class BackgroundSheetCrop : MonoBehaviour
    {
        enum Direction
        {
            LeftToRight,
            RightToLeft
        }

        [SerializeField] Material sheetMaterial;

        [Header("Game Progress")]
        [SerializeField, Range(0f, 1f)] float visibleProgress = 0f;

        [Header("Overscan")]
        [SerializeField] float hiddenLeft = -0.35f;
        [SerializeField] float hiddenRight = 1.35f;

        [Header("Look")]
        [SerializeField] Direction direction = Direction.LeftToRight;
        [SerializeField, Range(0f, 0.2f)] float softness = 0.03f;
        [SerializeField, Range(-1f, 1f)] float tilt = 0.25f;

        static readonly int ProgressId = Shader.PropertyToID("_Progress");
        static readonly int DirectionId = Shader.PropertyToID("_Direction");
        static readonly int SoftnessId = Shader.PropertyToID("_Softness");
        static readonly int TiltId = Shader.PropertyToID("_Tilt");

        Coroutine _fadeRoutine;

        void Awake()
        {
            Apply();
            G.BackgroundSheetCrop = this;
        }

        void OnValidate()
        {
            Apply();
        }

        public void SetVisibleProgress(float value)
        {
            visibleProgress = Mathf.Clamp01(value);
            Apply();
        }

        public void SetLeftToRight()
        {
            direction = Direction.LeftToRight;
            Apply();
        }

        public void SetRightToLeft()
        {
            direction = Direction.RightToLeft;
            Apply();
        }

        // =========================
        // ✨ FADE API
        // =========================

        public void FadeIn(float duration)
        {
            StartFade(1f, duration);
        }

        public void FadeOut(float duration)
        {
            StartFade(0f, duration);
        }

        void StartFade(float target, float duration)
        {
            if (_fadeRoutine != null)
                StopCoroutine(_fadeRoutine);

            _fadeRoutine = StartCoroutine(FadeRoutine(target, duration));
        }

        IEnumerator FadeRoutine(float target, float duration)
        {
            float start = visibleProgress;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;

                float t = time / duration;
                visibleProgress = Mathf.Lerp(start, target, t);

                Apply();
                yield return null;
            }

            visibleProgress = target;
            Apply();
        }

        // =========================

        void Apply()
        {
            if (sheetMaterial == null)
                return;

            float shaderProgress = Mathf.Lerp(hiddenLeft, hiddenRight, visibleProgress);

            sheetMaterial.SetFloat(ProgressId, shaderProgress);
            sheetMaterial.SetFloat(DirectionId, direction == Direction.LeftToRight ? 0f : 1f);
            sheetMaterial.SetFloat(SoftnessId, softness);
            sheetMaterial.SetFloat(TiltId, tilt);
        }
    }
}
