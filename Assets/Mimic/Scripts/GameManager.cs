using UnityEngine;

namespace Mimic.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public System.Action<float> OnScoreChanged;
        public System.Action<float> OnTotalScoreChanged;

        public System.Action<Color> OnTargetColorChanged;

        [SerializeField] GameSettings settings;
        [SerializeField] SpriteRenderer targetPreview;

        private Color _targetColor;

        public float RoundScore { get; private set; }
        public float TotalScore { get; private set; }

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
                    G.SharkMover.Reset();
                    G.OctopusMover.Reset();
                    G.BackgroundSheetCrop.FadeIn(settings.showTime, Vector2.left);
                    break;

                case RoundState.Guessing:
                    G.ColorPickerUI.SetInteractable(true);
                    G.OctopusMover.StartMoving(settings.guessTime / 3 * 2);
                    G.TimerUI.StartTimer(settings.guessTime);
                    break;

                case RoundState.Calculating:
                    G.ColorPickerUI.SetInteractable(false);
                    G.BackgroundSheetCrop.FadeOut(2f, Vector2.left);

                    CalculateRoundScore();
                    TotalScore += RoundScore;
                    OnScoreChanged?.Invoke(RoundScore);
                    OnTotalScoreChanged?.Invoke(TotalScore);
                    G.SharkMover.StartMoving(5f);
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

            RoundScore = Utils.CalculateScore(TargetColor, guessColor);

            Debug.Log($"Round Score: {RoundScore:0.00} | Total: {TotalScore:0.00}");
        }

        public void ResetScore()
        {
            RoundScore = 0f;
            TotalScore = 0f;

            OnScoreChanged?.Invoke(RoundScore);
            OnTotalScoreChanged?.Invoke(TotalScore);
        }
    }
}
