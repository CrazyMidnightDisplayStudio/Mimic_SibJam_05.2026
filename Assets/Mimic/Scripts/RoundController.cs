using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mimic.Scripts
{
    public enum RoundState
    {
        Showing,
        Guessing,
        Calculating
    }

    public class RoundController : MonoBehaviour
    {
        public System.Action<RoundState> OnRoundStateChanged;

        [SerializeField] GameSettings gameSettings;

        RoundState _state;
        Coroutine _loop;
        bool _continueRequested;

        void Awake()
        {
            G.RoundController = this;
        }
        void OnDestroy()
        {
            if (G.RoundController == this)
            {
                G.RoundController = null;
            }
        }

        void Update()
        {
            if (Keyboard.current?.rKey.wasPressedThisFrame == true)
            {
                StartRoundLoop();
            }
        }

        public void Continue()
        {
            _continueRequested = true;
        }

        public void StartRoundLoop()
        {
            Stop();
            _loop = StartCoroutine(RoundLoop());
        }

        public void Stop()
        {
            if (_loop != null)
            {
                StopCoroutine(_loop);
                _loop = null;
            }

            _continueRequested = false;
        }

        IEnumerator RoundLoop()
        {
            while (true)
            {
                SetState(RoundState.Showing);
                _continueRequested = false;
                yield return new WaitForSeconds(gameSettings.showTime);

                SetState(RoundState.Guessing);
                yield return new WaitForSeconds(gameSettings.guessTime);

                SetState(RoundState.Calculating);

                while (!_continueRequested)
                {
                    yield return null;
                }
            }
        }

        void SetState(RoundState newState)
        {
            Debug.Log($"RoundController new state: {newState}");
            _state = newState;
            OnRoundStateChanged?.Invoke(_state);
        }
    }
}
