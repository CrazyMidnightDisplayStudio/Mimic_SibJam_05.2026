using System.Collections;
using UnityEngine;

namespace Mimic.Scripts.UI
{
    public class BackgroundSheetCrop : MonoBehaviour
    {
        [SerializeField] Material sheetMaterial;

        [Header("Progress")]
        [SerializeField, Range(0f, 1f)] float visibleProgress = 0f;

        [Header("Direction")]
        [SerializeField] Vector2 direction = Vector2.right;

        [Header("Overscan")]
        [SerializeField] float overscan = 0.35f;

        [Header("Look")]
        [SerializeField, Range(0f, 0.3f)] float softness = 0.03f;
        [SerializeField, Range(-1f, 1f)] float tilt = 0.25f;

        static readonly int EdgeId = Shader.PropertyToID("_Edge");
        static readonly int DirectionId = Shader.PropertyToID("_Direction");
        static readonly int SoftnessId = Shader.PropertyToID("_Softness");
        static readonly int TiltId = Shader.PropertyToID("_Tilt");

        Coroutine _fadeRoutine;

        void Awake()
        {
            G.BackgroundSheetCrop = this;
            Apply();
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

        public void FadeIn(float duration, Vector2 fadeDirection)
        {
            direction = NormalizeDirection(fadeDirection);
            visibleProgress = 0f;
            Apply();

            StartFade(1f, duration);
        }

        public void FadeOut(float duration, Vector2 fadeDirection)
        {
            direction = NormalizeDirection(fadeDirection);
            visibleProgress = 1f;
            Apply();

            StartFade(0f, duration);
        }

        public void FadeIn(float duration)
        {
            FadeIn(duration, direction);
        }

        public void FadeOut(float duration)
        {
            FadeOut(duration, direction);
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

            if (duration <= 0f)
            {
                SetVisibleProgress(target);
                yield break;
            }

            while (time < duration)
            {
                time += Time.deltaTime;

                float t = Mathf.Clamp01(time / duration);
                t = Mathf.SmoothStep(0f, 1f, t);

                visibleProgress = Mathf.Lerp(start, target, t);
                Apply();

                yield return null;
            }

            visibleProgress = target;
            Apply();

            _fadeRoutine = null;
        }

        void Apply()
        {
            if (sheetMaterial == null)
                return;

            Vector2 dir = NormalizeDirection(direction);

            float minEdge;
            float maxEdge;
            CalculateEdgeRange(dir, tilt, out minEdge, out maxEdge);

            minEdge -= overscan;
            maxEdge += overscan;

            float edge = Mathf.Lerp(minEdge, maxEdge, visibleProgress);

            sheetMaterial.SetFloat(EdgeId, edge);
            sheetMaterial.SetVector(DirectionId, new Vector4(dir.x, dir.y, 0f, 0f));
            sheetMaterial.SetFloat(SoftnessId, softness);
            sheetMaterial.SetFloat(TiltId, tilt);
        }

        static Vector2 NormalizeDirection(Vector2 value)
        {
            if (value.sqrMagnitude < 0.001f)
                return Vector2.right;

            return value.normalized;
        }

        static void CalculateEdgeRange(Vector2 dir, float tilt, out float minEdge, out float maxEdge)
        {
            Vector2 perp = new Vector2(-dir.y, dir.x);

            Vector2[] corners =
            {
                new Vector2(-0.5f, -0.5f),
                new Vector2(-0.5f, 0.5f),
                new Vector2(0.5f, -0.5f),
                new Vector2(0.5f, 0.5f),
            };

            minEdge = float.PositiveInfinity;
            maxEdge = float.NegativeInfinity;

            foreach (Vector2 corner in corners)
            {
                float coord = Vector2.Dot(corner, dir) - Vector2.Dot(corner, perp) * tilt;

                minEdge = Mathf.Min(minEdge, coord);
                maxEdge = Mathf.Max(maxEdge, coord);
            }
        }
    }
}
