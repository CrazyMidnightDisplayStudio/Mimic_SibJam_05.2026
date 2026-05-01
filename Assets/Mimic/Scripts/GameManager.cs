using UnityEngine;
using UnityEngine.UI;

namespace Mimic.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public System.Action<float> OnScoreChanged;

        [SerializeField] GameSettings settings;
        [SerializeField] Image targetPreview;

        Color _targetColor;
        float _score;
        
        void Awake()
        {
            G.GameManager = this;
        }

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
            if (G.GameManager == this)
            {
                G.GameManager = null;
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
                    G.ColorPickerSlidersUI.SetInteractable(true);
                    break;

                case RoundState.Calculating:
                    G.ColorPickerSlidersUI.SetInteractable(false);
                    CalculateRoundScore();
                    OnScoreChanged?.Invoke(_score);
                    break;
            }
        }

        void RestartRound()
        {
            _targetColor = settings.GenerateTargetColor();
            targetPreview.color = _targetColor;
            G.TargetFinalImageUI.SetColor(_targetColor);

            G.ColorPickerSlidersUI.ResetPicker();
        }

        void CalculateRoundScore()
        {
            Color guessColor = G.ColorPickerSlidersUI.CurrentColor;

            _score = Utils.CalculateScore(_targetColor, guessColor);

            Debug.Log($"Score: {_score:0}");
        }
    }
}
