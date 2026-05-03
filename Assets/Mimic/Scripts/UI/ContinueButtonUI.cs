using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mimic.Scripts
{
    public class ContinueButtonUI : MonoBehaviour
    {
        [SerializeField] Button button;
        [SerializeField] TMP_Text buttonText;

        void Start()
        {
            button.gameObject.SetActive(false);
            button.onClick.AddListener(OnClick);
            G.RoundController.OnRoundStateChanged += HandleRoundStateChanged;
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
            bool isCalculating = state == RoundState.Calculating;
            button.gameObject.SetActive(isCalculating);

            if (!isCalculating)
                return;

            bool isLastRound = G.GameManager.CurrentRoundNumber >= G.GameManager.TotalRounds;

            buttonText.text = isLastRound ? "Finish" : "Next";
        }

        void OnClick()
        {
            G.RoundController.Continue();
        }
    }
}
