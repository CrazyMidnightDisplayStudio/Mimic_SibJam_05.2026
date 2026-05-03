using System.Collections.Generic;
using UnityEngine;

namespace Mimic.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public System.Action<float> OnScoreChanged;
        public System.Action<float> OnTotalScoreChanged;
        public System.Action<Color> OnTargetColorChanged;
        public System.Action<int, int> OnRoundChanged;

        [SerializeField] List<GameSettings> roundSettings = new();
        [SerializeField] SpriteRenderer targetPreview;

        Color _targetColor;
        int _currentRoundIndex;

        public float RoundScore { get; private set; }
        public float TotalScore { get; private set; }

        public bool GameFinished { get; private set; }
        public bool PlayerWon { get; private set; }

        public int CurrentRoundNumber => _currentRoundIndex + 1;
        public int TotalRounds => roundSettings.Count;

        public GameSettings CurrentSettings
        {
            get
            {
                if (roundSettings == null || roundSettings.Count == 0)
                    return null;

                int index = Mathf.Clamp(_currentRoundIndex, 0, roundSettings.Count - 1);
                return roundSettings[index];
            }
        }

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
        }

        void OnDestroy()
        {
            if (G.RoundController != null)
                G.RoundController.OnRoundStateChanged -= HandleRoundStateChanged;

            if (G.GameManager == this)
                G.GameManager = null;
        }

        void HandleRoundStateChanged(RoundState state)
        {
            switch (state)
            {
                case RoundState.Menu:
                    G.MainMenuUI.Show();
                    break;

                case RoundState.Showing:
                    StartRound(CurrentSettings);
                    break;

                case RoundState.Guessing:
                    StartGuessing(CurrentSettings);
                    break;

                case RoundState.Calculating:
                    FinishRound(CurrentSettings);
                    break;

                case RoundState.Win:
                    FinishGame(true);
                    break;

                case RoundState.Lose:
                    FinishGame(false);
                    break;
            }
        }

        void StartRound(GameSettings settings)
        {
            if (settings == null)
            {
                Debug.LogError("Round settings list is empty");
                return;
            }

            G.MainMenuUI.Hide();
            G.LeaderboardUI.Hide();

            OnRoundChanged?.Invoke(CurrentRoundNumber, TotalRounds);

            TargetColor = settings.GenerateTargetColor();
            targetPreview.color = TargetColor;

            G.ColorPickerUI.ResetPicker();

            G.SharkMover.Reset();
            G.OctopusMover.Reset();

            G.BackgroundSheetCrop.FadeIn(settings.showTime, Vector2.left);
        }

        void StartGuessing(GameSettings settings)
        {
            if (settings == null)
                return;

            G.ColorPickerUI.SetInteractable(true);
            G.OctopusMover.StartMoving(settings.guessTime / 3f * 2f);
            G.TimerUI.StartTimer(settings.guessTime);
        }

        void FinishRound(GameSettings settings)
        {
            if (settings == null)
                return;

            G.ColorPickerUI.SetInteractable(false);
            G.BackgroundSheetCrop.FadeOut(2f, Vector2.left);

            CalculateRoundScore();

            TotalScore += RoundScore;

            OnScoreChanged?.Invoke(RoundScore);
            OnTotalScoreChanged?.Invoke(TotalScore);

            G.SharkMover.StartMoving(5f);

            Debug.Log($"Round {CurrentRoundNumber}/{TotalRounds}: {RoundScore:0.00} | Total: {TotalScore:0.00}");

            if (RoundScore < settings.successThreshold)
            {
                GameFinished = true;
                PlayerWon = false;
                return;
            }

            if (_currentRoundIndex >= roundSettings.Count - 1)
            {
                GameFinished = true;
                PlayerWon = true;
                return;
            }

            GameFinished = false;
        }

        public void MoveToNextRound()
        {
            if (GameFinished)
                return;

            _currentRoundIndex++;
        }

        void FinishGame(bool won)
        {
            G.ColorPickerUI.SetInteractable(false);

            Debug.Log(won
                ? $"WIN. Total score: {TotalScore:0.00}"
                : $"LOSE. Total score: {TotalScore:0.00}");

            G.FinalMenuUI.Show(Mathf.RoundToInt(TotalScore), won);
        }

        void CalculateRoundScore()
        {
            Color guessColor = G.ColorPickerUI.CurrentColor;
            RoundScore = Utils.CalculateScore(TargetColor, guessColor);
        }

        public void ResetGame()
        {
            _currentRoundIndex = 0;

            RoundScore = 0f;
            TotalScore = 0f;

            GameFinished = false;
            PlayerWon = false;

            OnScoreChanged?.Invoke(RoundScore);
            OnTotalScoreChanged?.Invoke(TotalScore);
            OnRoundChanged?.Invoke(CurrentRoundNumber, TotalRounds);
        }
    }
}
