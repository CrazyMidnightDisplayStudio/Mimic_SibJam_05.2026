using UnityEngine;

public class SquashAnimator : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float squashAmount = 0.08f;
    [SerializeField] private float cycleDuration = 1.2f;
    [SerializeField] private bool affectWidth = true;
    [SerializeField] private bool playOnUnscaledTime;

    private Vector3 _baseScale;

    private void Awake()
    {
        if (target == null)
        {
            target = transform;
        }

        _baseScale = target.localScale;
    }

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        float duration = Mathf.Max(0.01f, cycleDuration);
        float time = playOnUnscaledTime ? Time.unscaledTime : Time.time;
        float wave = (Mathf.Sin(time * Mathf.PI * 2f / duration) + 1f) * 0.5f;
        float offset = Mathf.Lerp(-squashAmount, squashAmount, wave);

        Vector3 scale = _baseScale;

        if (affectWidth)
        {
            scale.x = _baseScale.x + offset;
            scale.y = _baseScale.y - offset;
        }
        else
        {
            scale.x = _baseScale.x - offset;
            scale.y = _baseScale.y + offset;
        }

        target.localScale = scale;
    }
}
