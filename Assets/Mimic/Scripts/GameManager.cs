using UnityEngine;
using UnityEngine.UI;

namespace Mimic.Scripts
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] GameSettings settings;
        [SerializeField] Image targetPreview;

        Color _targetColor;

        void Start()
        {
            G.RoundController.OnRoundStateChanged += HandleRoundStateChanged;
            G.RoundController.StartRoundLoop();
        }

        void OnDestroy()
        {
            if (G.RoundController != null)
            {
                G.RoundController.OnRoundStateChanged -= HandleRoundStateChanged;
            }
        }

        void HandleRoundStateChanged(RoundState state)
        {
            switch (state)
            {
                case RoundState.Showing:
                    RestartRound();
                    break;

                case RoundState.Guessing:
                    G.ColorPickerUI.SetInteractable(true);
                    break;

                case RoundState.Calculating:
                    G.ColorPickerUI.SetInteractable(false);
                    break;
            }
        }

        void RestartRound()
        {
            _targetColor = settings.GenerateTargetColor();
            targetPreview.color = _targetColor;

            G.ColorPickerUI.ResetPicker();
        }
    }
}
