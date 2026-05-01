using UnityEngine;
using UnityEngine.UI;

namespace Mimic.Scripts
{
    public class ContinueButtonUI : MonoBehaviour
    {
        [SerializeField] Button button;

        void Awake()
        {
            button.gameObject.SetActive(false);
            button.onClick.AddListener(OnClick);
        }

        void OnEnable()
        {
            G.RoundController.OnRoundStateChanged += HandleRoundStateChanged;
        }

        void OnDisable()
        {
            if (G.RoundController != null)
            {
                G.RoundController.OnRoundStateChanged -= HandleRoundStateChanged;
            }
        }

        void HandleRoundStateChanged(RoundState state)
        {
            button.gameObject.SetActive(state == RoundState.Calculating);
        }

        void OnClick()
        {
            G.RoundController.Continue();
        }
    }
}
