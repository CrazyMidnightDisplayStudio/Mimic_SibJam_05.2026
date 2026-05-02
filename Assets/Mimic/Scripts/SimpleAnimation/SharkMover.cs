using System.Collections;
using UnityEngine;

namespace Mimic.Scripts.SimpleAnimation
{
    public class SharkMover : MonoBehaviour
    {
        [SerializeField] float startX = -15f;
        [SerializeField] float endX = 15f;

        void Awake()
        {
            G.SharkMover = this;
        }

        void Start()
        {
            Reset();
        }

        public void StartMoving(float duration)
        {
            StartCoroutine(MoveShark(duration));
        }

        public void Reset()
        {
            Vector3 pos = transform.position;
            pos.x = startX;
            transform.position = pos;
        }


        IEnumerator MoveShark(float duration)
        {
            float time = 0f;

            Vector3 startPos = transform.position;
            Vector3 endPos = new Vector3(endX, startPos.y, startPos.z);

            while (time < duration)
            {
                float t = time / duration;
                transform.position = Vector3.Lerp(startPos, endPos, t);

                time += Time.deltaTime;
                yield return null;
            }

            transform.position = endPos; // чтобы точно дошла
        }
    }
}
