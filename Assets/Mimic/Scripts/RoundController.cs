using System.Collections;
using UnityEngine;

namespace Mimic.Scripts
{
    public enum RoundState
    {
        Initialization,
        Menu,
        Showing,
        Guessing,
        Calculating,
        Win,
        Lose
    }

    public class RoundController : MonoBehaviour
    {
        public System.Action<float> OnTimerProgressChanged;
        public System.Action<RoundState> OnRoundStateChanged;

        RoundState _state;
        Coroutine _loop;
        bool _continueRequested;
        bool _gameFinished;
        public RoundState CurrentState => _state;

        void Awake()
        {
            G.RoundController = this;
        }

        void Start()
        {
            SetState(RoundState.Initialization);
        }

        void OnDestroy()
        {
            if (G.RoundController == this)
                G.RoundController = null;
        }
        
        public void ShowMenu()
        {
            Stop();
            SetState(RoundState.Menu);
        }

        public void Play()
        {
            StartRoundLoop();
        }

        public void Continue()
        {
            _continueRequested = true;
        }

        public void StartRoundLoop()
        {
            Stop();

            _gameFinished = false;
            _continueRequested = false;

            G.GameManager.ResetGame();

            _loop = StartCoroutine(RoundLoop());
        }

        public void FinishGame(bool won)
        {
            _gameFinished = true;
            SetState(won ? RoundState.Win : RoundState.Lose);
        }

        public void Stop()
        {
            if (_loop != null)
            {
                StopCoroutine(_loop);
                _loop = null;
            }

            _continueRequested = false;
            _gameFinished = false;
        }

        IEnumerator RoundLoop()
        {
            while (!_gameFinished)
            {
                GameSettings settings = G.GameManager.CurrentSettings;

                SetState(RoundState.Showing);
                _continueRequested = false;
                yield return WaitWithProgress(settings.showTime);

                SetState(RoundState.Guessing);
                yield return WaitWithProgress(settings.guessTime);

                SetState(RoundState.Calculating);

                _continueRequested = false;

                while (!_continueRequested && !_gameFinished)
                    yield return null;

                if (_gameFinished)
                    break;

                if (G.GameManager.GameFinished)
                {
                    FinishGame(G.GameManager.PlayerWon);
                }
                else
                {
                    G.GameManager.MoveToNextRound();
                }
            }

            _loop = null;
        }

        IEnumerator WaitWithProgress(float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float progress = 1f - Mathf.Clamp01(elapsed / duration);
                OnTimerProgressChanged?.Invoke(progress);

                yield return null;
            }

            OnTimerProgressChanged?.Invoke(0f);
        }

        void SetState(RoundState newState)
        {
            Debug.Log($"RoundController new state: {newState}");
            _state = newState;
            OnRoundStateChanged?.Invoke(_state);
        }
    }
}
