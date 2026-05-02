using System.Collections;
using UnityEngine;

namespace Mimic.Scripts.SimpleAnimation
{
    public class OctopusMover : MonoBehaviour
    {
        [SerializeField] float startY = 15f;
        [SerializeField] float endY = -15f;
        [SerializeField] FloatingScaleAnimator octopusFloating;
        float _startedFloating = 0f;

        void Awake()
        {
            G.OctopusMover = this;
            _startedFloating = octopusFloating.FloatDistance;
        }

        void Start()
        {
            Reset();
        }

        public void StartMoving(float duration)
        {
            StartCoroutine(MoveOctopus(duration));
        }

        public void Reset()
        {
            Vector3 pos = transform.position;
            pos.y = startY;
            transform.position = pos;
            octopusFloating.FloatDistance = _startedFloating;
        }

        IEnumerator MoveOctopus(float duration)
        {
            float time = 0f;

            Vector3 startPos = transform.position;
            Vector3 endPos = new Vector3(startPos.x, endY, startPos.z);

            while (time < duration)
            {
                float t = time / duration;
                transform.position = Vector3.Lerp(startPos, endPos, t);

                time += Time.deltaTime;
                yield return null;
            }

            transform.position = endPos;
            octopusFloating.FloatDistance = 0;
        }
    }
}
