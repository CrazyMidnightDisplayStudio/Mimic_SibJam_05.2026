using TMPro;
using UnityEngine;

namespace Mimic.Scripts.UI
{
    public class RoundCounterUI : MonoBehaviour
    {
        [SerializeField] TMP_Text roundText;

        void Awake()
        {
            G.RoundCounterUI = this;
        }

        void Start()
        {
            if (G.GameManager != null)
                G.GameManager.OnRoundChanged += UpdateText;

            if (G.RoundController != null)
                G.RoundController.OnRoundStateChanged += HandleState;

            // сразу обновляем
            UpdateText(G.GameManager.CurrentRoundNumber, G.GameManager.TotalRounds);
            HandleState(G.RoundController.CurrentState);
        }

        void OnDestroy()
        {
            if (G.GameManager != null)
                G.GameManager.OnRoundChanged -= UpdateText;

            if (G.RoundController != null)
                G.RoundController.OnRoundStateChanged -= HandleState;

            if (G.RoundCounterUI == this)
                G.RoundCounterUI = null;
        }

        void UpdateText(int current, int total)
        {
            roundText.text = $"Round {current}/{total}";
        }

        void HandleState(RoundState state)
        {
            bool visible =
                state == RoundState.Showing ||
                state == RoundState.Guessing ||
                state == RoundState.Calculating;

            roundText.gameObject.SetActive(visible);
        }
    }
}
