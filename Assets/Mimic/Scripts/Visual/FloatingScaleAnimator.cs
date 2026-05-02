using UnityEngine;

public sealed class FloatingScaleAnimator : MonoBehaviour
{
    [SerializeField] private Transform floatingTarget;
    [SerializeField] private Transform scalingTarget;
    [SerializeField] private float cycleDuration = 2f;
    [SerializeField] private float scaleAmount = 0.08f;
    [SerializeField] private bool playOnUnscaledTime;
    public float FloatDistance = 0.25f;

    private Vector3 _floatingStartPosition;
    private Vector3 _scalingStartScale;
    private float _time;

    private void Awake()
    {
        if (floatingTarget != null)
        {
            _floatingStartPosition = floatingTarget.localPosition;
        }

        if (scalingTarget != null)
        {
            _scalingStartScale = scalingTarget.localScale;
        }
    }

    private void OnEnable()
    {
        _time = 0f;

        if (floatingTarget != null)
        {
            _floatingStartPosition = floatingTarget.localPosition;
        }

        if (scalingTarget != null)
        {
            _scalingStartScale = scalingTarget.localScale;
        }
    }

    private void Update()
    {
        if (floatingTarget == null || scalingTarget == null)
        {
            return;
        }

        if (cycleDuration <= 0f)
        {
            return;
        }

        float deltaTime = playOnUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        _time += deltaTime;

        float normalizedTime = _time / cycleDuration;
        float wave = Mathf.Sin(normalizedTime * Mathf.PI * 2f);

        floatingTarget.localPosition = _floatingStartPosition + Vector3.up * (wave * FloatDistance);

        float inverseWave = -wave;
        float scaleMultiplier = 1f + inverseWave * scaleAmount;
        scalingTarget.localScale = _scalingStartScale * scaleMultiplier;
    }
}
