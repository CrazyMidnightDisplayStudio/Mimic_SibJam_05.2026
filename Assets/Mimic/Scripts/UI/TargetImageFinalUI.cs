using UnityEngine;
using UnityEngine.UI;

namespace Mimic.Scripts.UI
{
    public class TargetFinalImageUI : MonoBehaviour
    {
        [SerializeField] Image image;

        void Awake()
        {
            G.TargetFinalImageUI = this;
            image.gameObject.SetActive(false);
        }

        void Start()
        {
            G.RoundController.OnRoundStateChanged += HandleState;
        }

        void OnDestroy()
        {
            if (G.RoundController != null)
            {
                G.RoundController.OnRoundStateChanged -= HandleState;
            }

            if (G.TargetFinalImageUI == this)
            {
                G.TargetFinalImageUI = null;
            }
        }

        public void SetColor(Color color)
        {
            image.color = color;
        }

        void HandleState(RoundState state)
        {
            image.gameObject.SetActive(state == RoundState.Calculating);
        }
    }
}
