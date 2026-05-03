using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayFab.ClientModels;
using UnityEngine;

namespace Mimic.Scripts.UI.Leaderboard
{
    public class LeaderboardUI : MonoBehaviour
    {
        [SerializeField] GameObject root;

        [Header("Rows")]
        [SerializeField] RectTransform rowsRoot;
        [SerializeField] LeaderboardRowUI rowPrefab;

        [Header("Settings")]
        [SerializeField] int maxResults = 10;
        [SerializeField] float rowHeight = 48f;
        [SerializeField] float rowSpacing = 8f;
        [SerializeField] float rowAnimationDuration = 0.35f;
        [SerializeField] float rowAnimationDelay = 0.06f;
        [SerializeField] float insertDelay = 0.35f;
        [SerializeField] float bottomOffset = 160f;

        readonly List<LeaderboardRowUI> _rows = new();

        Coroutine _animationRoutine;

        void Awake()
        {
            G.LeaderboardUI = this;
            root.SetActive(false);
        }

        void OnDestroy()
        {
            if (G.LeaderboardUI == this)
                G.LeaderboardUI = null;
        }

        public void Show(bool animateInsertion = false)
        {
            root.SetActive(true);
            LoadAndShowLeaderboard(animateInsertion);
        }

        public void Hide()
        {
            root.SetActive(false);
        }
        public void ShowImproved(
            PlayerLeaderboardEntry previousEntry,
            PlayerLeaderboardEntry newEntry)
        {
            root.SetActive(true);

            G.LeaderboardService.LoadLeaderboardWithCurrentPlayer(
                maxResults,
                (topEntries, currentPlayer) =>
                {
                    if (_animationRoutine != null)
                        StopCoroutine(_animationRoutine);

                    _animationRoutine = StartCoroutine(
                        BuildImprovedAnimation(topEntries, previousEntry, newEntry));
                });
        }

        void LoadAndShowLeaderboard(bool animateInsertion)
        {
            G.LeaderboardService.LoadLeaderboardWithCurrentPlayer(
                maxResults,
                (topEntries, currentPlayer) =>
                {
                    if (_animationRoutine != null)
                        StopCoroutine(_animationRoutine);

                    _animationRoutine = StartCoroutine(
                        animateInsertion
                            ? BuildWithInsertionAnimation(topEntries, currentPlayer)
                            : BuildWithAppearAnimation(topEntries, currentPlayer));
                });
        }

        IEnumerator BuildWithAppearAnimation(
            List<PlayerLeaderboardEntry> topEntries,
            PlayerLeaderboardEntry currentPlayer)
        {
            ClearRows();

            List<PlayerLeaderboardEntry> entries = BuildVisibleEntries(topEntries, currentPlayer);

            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                var row = CreateRow();

                Vector2 targetPosition = GetRowPosition(i);
                bool isCurrentPlayer = IsCurrentPlayer(entry);

                row.SetData(
                    entry.Position + 1,
                    GetDisplayName(entry),
                    entry.StatValue,
                    isCurrentPlayer);

                _rows.Add(row);

                yield return row.AnimateFromBottom(
                    targetPosition,
                    bottomOffset,
                    rowAnimationDuration);

                yield return new WaitForSeconds(rowAnimationDelay);
            }
        }

        IEnumerator BuildWithInsertionAnimation(
            List<PlayerLeaderboardEntry> topEntries,
            PlayerLeaderboardEntry currentPlayer)
        {
            ClearRows();

            if (currentPlayer == null || topEntries == null)
            {
                yield return BuildWithAppearAnimation(topEntries, currentPlayer);
                yield break;
            }

            bool currentInTop = topEntries.Any(IsCurrentPlayer);

            if (!currentInTop)
            {
                yield return BuildWithAppearAnimation(topEntries, currentPlayer);
                yield break;
            }

            int currentFinalIndex = topEntries.FindIndex(IsCurrentPlayer);

            List<PlayerLeaderboardEntry> entriesWithoutCurrent = topEntries
                .Where(entry => !IsCurrentPlayer(entry))
                .ToList();

            for (int i = 0; i < entriesWithoutCurrent.Count; i++)
            {
                var entry = entriesWithoutCurrent[i];
                var row = CreateRow();

                row.SetData(
                    entry.Position + 1,
                    GetDisplayName(entry),
                    entry.StatValue,
                    false);

                row.SetPosition(GetRowPosition(i));
                _rows.Add(row);
            }

            yield return new WaitForSeconds(insertDelay);

            for (int i = 0; i < _rows.Count; i++)
            {
                int targetIndex = i >= currentFinalIndex ? i + 1 : i;
                StartCoroutine(_rows[i].MoveTo(GetRowPosition(targetIndex), rowAnimationDuration));
            }

            var currentRow = CreateRow();

            currentRow.SetData(
                currentPlayer.Position + 1,
                GetDisplayName(currentPlayer),
                currentPlayer.StatValue,
                true);

            _rows.Insert(currentFinalIndex, currentRow);

            yield return currentRow.AnimateFromBottom(
                GetRowPosition(currentFinalIndex),
                bottomOffset,
                rowAnimationDuration);
        }

        List<PlayerLeaderboardEntry> BuildVisibleEntries(
            List<PlayerLeaderboardEntry> topEntries,
            PlayerLeaderboardEntry currentPlayer)
        {
            var result = new List<PlayerLeaderboardEntry>();

            if (topEntries != null)
                result.AddRange(topEntries);

            bool currentAlreadyVisible = result.Any(IsCurrentPlayer);

            if (!currentAlreadyVisible && currentPlayer != null)
                result.Add(currentPlayer);

            return result;
        }

        LeaderboardRowUI CreateRow()
        {
            var row = Instantiate(rowPrefab, rowsRoot);
            row.gameObject.SetActive(true);
            return row;
        }

        void ClearRows()
        {
            foreach (Transform child in rowsRoot)
                Destroy(child.gameObject);

            _rows.Clear();
        }

        Vector2 GetRowPosition(int index)
        {
            float y = -index * (rowHeight + rowSpacing);
            return new Vector2(0f, y);
        }

        bool IsCurrentPlayer(PlayerLeaderboardEntry entry)
        {
            return entry != null &&
                entry.PlayFabId == G.LeaderboardService.CurrentPlayFabId;
        }

        string GetDisplayName(PlayerLeaderboardEntry entry)
        {
            if (entry == null || string.IsNullOrEmpty(entry.DisplayName))
                return "Player";

            return entry.DisplayName;
        }
        IEnumerator BuildImprovedAnimation(
            List<PlayerLeaderboardEntry> topEntries,
            PlayerLeaderboardEntry previousEntry,
            PlayerLeaderboardEntry newEntry)
        {
            bool newInTop = topEntries != null && topEntries.Any(IsCurrentPlayer);

            if (newInTop)
            {
                yield return BuildWithInsertionAnimation(topEntries, newEntry);
                yield break;
            }

            // Игрок всё ещё ниже топ-10, но его 11-я строка изменилась.
            yield return BuildWithAppearAnimation(topEntries, previousEntry);

            LeaderboardRowUI playerRow = _rows.LastOrDefault();

            if (playerRow != null && previousEntry != null && newEntry != null)
            {
                yield return playerRow.AnimatePlaceAndScore(
                    previousEntry.Position + 1,
                    newEntry.Position + 1,
                    previousEntry.StatValue,
                    newEntry.StatValue,
                    1.2f);
            }
        }
    }
}
