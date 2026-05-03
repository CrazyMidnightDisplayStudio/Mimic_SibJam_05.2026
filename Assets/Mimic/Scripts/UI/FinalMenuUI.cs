using System.Collections;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mimic.Scripts.UI
{
    public class FinalMenuUI : MonoBehaviour
    {
        [SerializeField] GameObject root;
        [SerializeField] TMP_Text titleText;
        [SerializeField] TMP_Text scoreText;
        [SerializeField] Button playAgainButton;

        [Header("Animation")]
        [SerializeField] float rollDuration = 2f;
        [SerializeField] AnimationCurve rollCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        Coroutine _rollCoroutine;

        void Awake()
        {
            G.FinalMenuUI = this;

            root.SetActive(false);
            playAgainButton.onClick.AddListener(PlayAgain);
        }

        void OnDestroy()
        {
            playAgainButton.onClick.RemoveListener(PlayAgain);

            if (G.FinalMenuUI == this)
                G.FinalMenuUI = null;
        }

        public void Show(int score, bool won)
        {
            root.SetActive(true);
            playAgainButton.gameObject.SetActive(false);

            titleText.text = won ? "You Win!" : "Game Over";
            scoreText.text = "";

            G.LeaderboardUI.Hide();

            G.LeaderboardService.GetCurrentPlayerEntry(previousEntry =>
            {
                int previousBest = previousEntry?.StatValue ?? 0;

                if (_rollCoroutine != null)
                    StopCoroutine(_rollCoroutine);

                _rollCoroutine = StartCoroutine(FinalFlow(score, previousBest, previousEntry));
            });
        }

        IEnumerator FinalFlow(int score, int previousBest, PlayerLeaderboardEntry previousEntry)
        {
            int effectiveBest = previousEntry == null
                ? score
                : Mathf.Max(previousBest, score);

            yield return RollScore(score, effectiveBest);

            G.LeaderboardService.SubmitCurrentPlayerScoreIfBetter(score,
                (improved, previousEntry, newEntry) =>
                {
                    if (improved)
                        G.LeaderboardUI.ShowImproved(previousEntry, newEntry);
                    else
                        G.LeaderboardUI.Show();

                    playAgainButton.gameObject.SetActive(true);
                });
        }

        IEnumerator RollScore(int score, int previousBest)
        {
            float time = 0f;

            while (time < rollDuration)
            {
                time += Time.deltaTime;

                float t = Mathf.Clamp01(time / rollDuration);
                float curvedT = rollCurve.Evaluate(t);

                int shownScore = Mathf.RoundToInt(Mathf.Lerp(0, score, curvedT));
                int shownBest = Mathf.RoundToInt(Mathf.Lerp(0, previousBest, curvedT));

                scoreText.text = $"Score: {shownScore} | Your best: {shownBest}";

                yield return null;
            }

            scoreText.text = $"Score: {score} | Your best: {previousBest}";
        }

        void PlayAgain()
        {
            root.SetActive(false);
            G.LeaderboardUI.Hide();
            G.RoundController.Play();
        }
    }
}
