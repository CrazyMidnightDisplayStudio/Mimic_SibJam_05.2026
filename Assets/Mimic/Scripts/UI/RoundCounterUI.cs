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

            // чтобы сразу показать при старте
            UpdateText(G.GameManager.CurrentRoundNumber, G.GameManager.TotalRounds);
        }

        void OnDestroy()
        {
            if (G.GameManager != null)
                G.GameManager.OnRoundChanged -= UpdateText;

            if (G.RoundCounterUI == this)
                G.RoundCounterUI = null;
        }

        void UpdateText(int current, int total)
        {
            roundText.text = $"Round {current}/{total}";
        }
    }
}
