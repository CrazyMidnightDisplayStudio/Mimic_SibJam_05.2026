using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Mimic.Scripts
{
    public class LeaderboardService : MonoBehaviour
    {
        const string PlayerIdKey = "Mimic_PlayerId";
        const string StatisticName = "Score";

        public string CurrentPlayFabId { get; private set; }
        public string CurrentPlayerName { get; private set; }
        public bool IsLoggedIn { get; private set; }

        void Awake()
        {
            G.LeaderboardService = this;
        }

        void Start()
        {
            Login();
        }

        void OnDestroy()
        {
            if (G.LeaderboardService == this)
                G.LeaderboardService = null;
        }

        public void Login()
        {
            PlayFabClientAPI.LoginWithCustomID(
                new LoginWithCustomIDRequest
                {
                    CustomId = GetOrCreatePlayerId(),
                    CreateAccount = true
                },
                result =>
                {
                    IsLoggedIn = true;
                    CurrentPlayFabId = result.PlayFabId;

                    Debug.Log("PlayFab login success");

                    GetAccountInfo(() =>
                    {
                        G.RoundController.ShowMenu();
                    });
                },
                error =>
                {
                    IsLoggedIn = false;
                    Debug.LogError(error.GenerateErrorReport());
                });
        }

        public void SetPlayerName(string playerName, Action onComplete = null)
        {
            if (!IsLoggedIn)
            {
                Debug.LogWarning("Cannot set name: PlayFab is not logged in yet");
                onComplete?.Invoke();
                return;
            }

            SetDisplayName(playerName, onComplete);
        }

        public void SubmitScore(string playerName, int score, Action onComplete = null)
        {
            if (!IsLoggedIn)
            {
                Debug.LogWarning("Cannot submit score: PlayFab is not logged in yet");
                onComplete?.Invoke();
                return;
            }

            SetDisplayName(playerName, () => SendScore(score, onComplete));
        }

        public void LoadLeaderboard(int maxResults, Action<List<PlayerLeaderboardEntry>> onLoaded)
        {
            PlayFabClientAPI.GetLeaderboard(
                new GetLeaderboardRequest
                {
                    StatisticName = StatisticName,
                    StartPosition = 0,
                    MaxResultsCount = maxResults
                },
                result =>
                {
                    onLoaded?.Invoke(result.Leaderboard);
                },
                error =>
                {
                    Debug.LogError(error.GenerateErrorReport());
                    onLoaded?.Invoke(new List<PlayerLeaderboardEntry>());
                });
        }

        public void LoadLeaderboard(Action<List<PlayerLeaderboardEntry>> onLoaded)
        {
            LoadLeaderboard(10, onLoaded);
        }

        public void LoadLeaderboardWithCurrentPlayer(
            int maxResults,
            Action<List<PlayerLeaderboardEntry>, PlayerLeaderboardEntry> onLoaded)
        {
            PlayFabClientAPI.GetLeaderboard(
                new GetLeaderboardRequest
                {
                    StatisticName = StatisticName,
                    StartPosition = 0,
                    MaxResultsCount = maxResults
                },
                topResult =>
                {
                    PlayFabClientAPI.GetLeaderboardAroundPlayer(
                        new GetLeaderboardAroundPlayerRequest
                        {
                            StatisticName = StatisticName,
                            MaxResultsCount = 1
                        },
                        currentResult =>
                        {
                            PlayerLeaderboardEntry currentPlayer = null;

                            if (currentResult.Leaderboard != null && currentResult.Leaderboard.Count > 0)
                                currentPlayer = currentResult.Leaderboard[0];

                            onLoaded?.Invoke(topResult.Leaderboard, currentPlayer);
                        },
                        error =>
                        {
                            Debug.LogError(error.GenerateErrorReport());
                            onLoaded?.Invoke(topResult.Leaderboard, null);
                        });
                },
                error =>
                {
                    Debug.LogError(error.GenerateErrorReport());
                    onLoaded?.Invoke(new List<PlayerLeaderboardEntry>(), null);
                });
        }

        void SetDisplayName(string playerName, Action onSuccess)
        {
            playerName = NormalizeName(playerName);

            PlayFabClientAPI.UpdateUserTitleDisplayName(
                new UpdateUserTitleDisplayNameRequest
                {
                    DisplayName = playerName
                },
                result =>
                {
                    CurrentPlayerName = result.DisplayName;

                    Debug.Log($"Display name set: {CurrentPlayerName}");

                    onSuccess?.Invoke();
                },
                error =>
                {
                    Debug.LogError(error.GenerateErrorReport());
                });
        }

        void SendScore(int score, Action onComplete)
        {
            PlayFabClientAPI.UpdatePlayerStatistics(
                new UpdatePlayerStatisticsRequest
                {
                    Statistics = new List<StatisticUpdate>
                    {
                        new StatisticUpdate
                        {
                            StatisticName = StatisticName,
                            Value = score
                        }
                    }
                },
                result =>
                {
                    Debug.Log($"Score sent: {score}");
                    onComplete?.Invoke();
                },
                error =>
                {
                    Debug.LogError(error.GenerateErrorReport());
                    onComplete?.Invoke();
                });
        }

        void GetAccountInfo(System.Action onComplete = null)
        {
            PlayFabClientAPI.GetAccountInfo(
                new GetAccountInfoRequest(),
                result =>
                {
                    CurrentPlayerName = result.AccountInfo.TitleInfo.DisplayName;
                    Debug.Log($"Player name: {CurrentPlayerName}");

                    onComplete?.Invoke(); // ← ВАЖНО
                },
                error =>
                {
                    Debug.LogError(error.GenerateErrorReport());

                    onComplete?.Invoke(); // ← даже при ошибке идём дальше
                });
        }

        string GetOrCreatePlayerId()
        {
            if (PlayerPrefs.HasKey(PlayerIdKey))
                return PlayerPrefs.GetString(PlayerIdKey);

            string id = Guid.NewGuid().ToString();

            PlayerPrefs.SetString(PlayerIdKey, id);
            PlayerPrefs.Save();

            return id;
        }

        string NormalizeName(string playerName)
        {
            playerName = playerName.Trim();

            if (playerName.Length < 3)
                playerName = "Player";

            if (playerName.Length > 25)
                playerName = playerName.Substring(0, 25);

            return playerName;
        }
        public void GetCurrentPlayerEntry(Action<PlayerLeaderboardEntry> onLoaded)
        {
            PlayFabClientAPI.GetLeaderboardAroundPlayer(
                new GetLeaderboardAroundPlayerRequest
                {
                    StatisticName = StatisticName,
                    MaxResultsCount = 1
                },
                result =>
                {
                    PlayerLeaderboardEntry entry = null;

                    if (result.Leaderboard != null && result.Leaderboard.Count > 0)
                        entry = result.Leaderboard[0];

                    onLoaded?.Invoke(entry);
                },
                error =>
                {
                    Debug.LogError(error.GenerateErrorReport());
                    onLoaded?.Invoke(null);
                });
        }

        public void SubmitCurrentPlayerScoreIfBetter(
            int score,
            Action<bool, PlayerLeaderboardEntry, PlayerLeaderboardEntry> onComplete = null)
        {
            GetCurrentPlayerEntry(previousEntry =>
            {
                int previousBest = previousEntry?.StatValue ?? 0;

                if (previousEntry != null && score <= previousBest)
                {
                    Debug.Log($"Score not improved. Current: {score}, Best: {previousBest}");
                    onComplete?.Invoke(false, previousEntry, previousEntry);
                    return;
                }

                SubmitScore(CurrentPlayerName, score, () =>
                {
                    StartCoroutine(WaitForUpdatedEntry(score, previousEntry, onComplete));
                });
            });
        }

        IEnumerator WaitForUpdatedEntry(
            int expectedScore,
            PlayerLeaderboardEntry previousEntry,
            Action<bool, PlayerLeaderboardEntry, PlayerLeaderboardEntry> onComplete)
        {
            const int maxAttempts = 8;
            const float delay = 0.35f;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                bool finished = false;
                PlayerLeaderboardEntry entry = null;

                GetCurrentPlayerEntry(result =>
                {
                    entry = result;
                    finished = true;
                });

                yield return new WaitUntil(() => finished);

                if (entry != null && entry.StatValue >= expectedScore)
                {
                    Debug.Log($"Updated leaderboard entry received: {entry.StatValue}");
                    onComplete?.Invoke(true, previousEntry, entry);
                    yield break;
                }

                yield return new WaitForSeconds(delay);
            }

            Debug.LogWarning("Leaderboard did not update in time, using expected score fallback");

            onComplete?.Invoke(true, previousEntry, previousEntry);
        }
    }
}
