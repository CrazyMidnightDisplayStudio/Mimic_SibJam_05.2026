using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mimic.Scripts.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] GameObject root;

        [Header("Name")]
        [SerializeField] TMP_Text namePreviewText;
        [SerializeField] Button changeNameButton;

        [Header("Buttons")]
        [SerializeField] Button playButton;

        string _selectedName;
        bool _nameAlreadySaved;

        void Awake()
        {
            G.MainMenuUI = this;

            root.SetActive(false);

            changeNameButton.onClick.AddListener(ChangeName);
            playButton.onClick.AddListener(Play);
        }

        void OnDestroy()
        {
            changeNameButton.onClick.RemoveListener(ChangeName);
            playButton.onClick.RemoveListener(Play);

            if (G.MainMenuUI == this)
                G.MainMenuUI = null;
        }

        public void Show()
        {
            root.SetActive(true);

            string savedName = G.LeaderboardService.CurrentPlayerName;
            _nameAlreadySaved = !string.IsNullOrEmpty(savedName);

            if (_nameAlreadySaved)
                _selectedName = savedName;
            else
                _selectedName = Utils.GetRandomName();

            UpdateNameView();

            changeNameButton.gameObject.SetActive(!_nameAlreadySaved);
            namePreviewText.gameObject.SetActive(!_nameAlreadySaved);
            playButton.interactable = true;

            G.LeaderboardUI.Show();
        }

        public void Hide()
        {
            root.SetActive(false);
        }

        void ChangeName()
        {
            if (_nameAlreadySaved)
                return;

            _selectedName = Utils.GetRandomName();
            UpdateNameView();
        }

        void Play()
        {
            playButton.interactable = false;
            changeNameButton.interactable = false;

            if (_nameAlreadySaved)
            {
                StartGame();
                return;
            }

            G.LeaderboardService.SetPlayerName(_selectedName, () =>
            {
                _nameAlreadySaved = true;
                StartGame();
            });
        }

        void StartGame()
        {
            Hide();
            G.LeaderboardUI.Hide();

            playButton.interactable = true;
            changeNameButton.interactable = true;

            G.RoundController.Play();
        }

        void UpdateNameView()
        {
            namePreviewText.text = _selectedName;
        }
    }
}
