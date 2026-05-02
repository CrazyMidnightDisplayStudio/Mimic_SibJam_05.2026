using UnityEngine;

namespace Mimic.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public System.Action<float> OnScoreChanged;
        public System.Action<Color> OnTargetColorChanged;

        [SerializeField] GameSettings settings;
        [SerializeField] SpriteRenderer targetPreview;

        private Color _targetColor;

        public float Score { get; private set; }

        public Color TargetColor
        {
            get => _targetColor;
            private set
            {
                _targetColor = value;
                OnTargetColorChanged?.Invoke(value);
            }
        }

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
                    G.BackgroundSheetCrop.FadeIn(settings.showTime, Vector2.left);
                    break;

                case RoundState.Guessing:
                    G.ColorPickerUI.SetInteractable(true);
                    break;

                case RoundState.Calculating:
                    G.ColorPickerUI.SetInteractable(false);
                    G.BackgroundSheetCrop.FadeOut(2f, Vector2.left);
                    CalculateRoundScore();
                    OnScoreChanged?.Invoke(Score);
                    break;
            }
        }

        void RestartRound()
        {
            TargetColor = settings.GenerateTargetColor();
            targetPreview.color = TargetColor;

            G.ColorPickerUI.ResetPicker();
        }

        void CalculateRoundScore()
        {
            Color guessColor = G.ColorPickerUI.CurrentColor;

            Score = Utils.CalculateScore(TargetColor, guessColor);

            Debug.Log($"Score: {Score:0}");
        }
    }
}
