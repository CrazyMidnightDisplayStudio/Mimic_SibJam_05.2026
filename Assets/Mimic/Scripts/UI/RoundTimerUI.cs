using UnityEngine;
using UnityEngine.UI;

namespace Mimic.Scripts.UI
{
    public class RoundTimerUI : MonoBehaviour
    {
        [SerializeField] Slider slider;

        void Start()
        {
            G.RoundController.OnRoundStateChanged += HandleState;
            G.RoundController.OnTimerProgressChanged += HandleProgress;

            slider.gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            if (G.RoundController != null)
            {
                G.RoundController.OnRoundStateChanged -= HandleState;
                G.RoundController.OnTimerProgressChanged -= HandleProgress;
            }
        }

        void HandleState(RoundState state)
        {
            bool visible = state == RoundState.Showing || state == RoundState.Guessing;
            slider.gameObject.SetActive(visible);

            if (visible)
            {
                slider.value = 1f;
            }
        }

        void HandleProgress(float progress)
        {
            slider.value = progress;
        }
    }
}
