using UnityEngine;
using TMPro;

namespace Mimic.Scripts.UI
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] TMP_Text scoreText;

        void Awake()
        {
            G.ScoreUI = this;
            scoreText.gameObject.SetActive(false);
        }

        void Start()
        {
            G.RoundController.OnRoundStateChanged += HandleState;
            G.GameManager.OnScoreChanged += HandleScoreChanged;
        }

        void OnDestroy()
        {
            if (G.RoundController != null)
            {
                G.RoundController.OnRoundStateChanged -= HandleState;
            }

            if (G.GameManager != null)
            {
                G.GameManager.OnScoreChanged -= HandleScoreChanged;
            }

            if (G.ScoreUI == this)
            {
                G.ScoreUI = null;
            }
        }

        void HandleScoreChanged(float score)
        {
            scoreText.text = $"{score:0.0}";
        }

        void HandleState(RoundState state)
        {
            scoreText.gameObject.SetActive(state == RoundState.Calculating);
        }
    }
}
