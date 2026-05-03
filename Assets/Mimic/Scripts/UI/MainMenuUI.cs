using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mimic.Scripts.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] GameObject root;

        [SerializeField] TMP_InputField nameInput;
        [SerializeField] Button saveNameButton;
        [SerializeField] Button playButton;

        void Awake()
        {
            G.MainMenuUI = this;

            root.SetActive(false);

            saveNameButton.onClick.AddListener(SaveName);
            playButton.onClick.AddListener(Play);
        }

        void OnDestroy()
        {
            saveNameButton.onClick.RemoveListener(SaveName);
            playButton.onClick.RemoveListener(Play);

            if (G.MainMenuUI == this)
                G.MainMenuUI = null;
        }

        public void Show()
        {
            root.SetActive(true);

            bool hasName = !string.IsNullOrEmpty(G.LeaderboardService.CurrentPlayerName);

            nameInput.gameObject.SetActive(!hasName);
            saveNameButton.gameObject.SetActive(!hasName);

            if (hasName)
                nameInput.text = G.LeaderboardService.CurrentPlayerName;

            // показываем лидерборд
            G.LeaderboardUI.Show();
        }

        public void Hide()
        {
            root.SetActive(false);
        }

        void SaveName()
        {
            saveNameButton.interactable = false;

            G.LeaderboardService.SetPlayerName(nameInput.text, () =>
            {
                saveNameButton.interactable = true;

                nameInput.gameObject.SetActive(false);
                saveNameButton.gameObject.SetActive(false);

                G.LeaderboardUI.Show(); // обновить таблицу
            });
        }

        void Play()
        {
            Hide();
            G.LeaderboardUI.Hide();

            G.RoundController.Play();
        }
    }
}
