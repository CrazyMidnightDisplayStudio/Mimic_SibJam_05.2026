using UnityEngine;
using UnityEngine.UI;

namespace Mimic.Scripts.UI
{
    public class GuessImageUI : MonoBehaviour
    {
        [SerializeField] Image image;

        void Awake()
        {
            G.GuessImageUI = this;
        }
        void Start()
        {
            if (G.RoundController != null)
            {
                G.RoundController.OnRoundStateChanged += HandleState;
            }
        }

        void OnDestroy()
        {
            if (G.RoundController != null)
            {
                G.RoundController.OnRoundStateChanged -= HandleState;
            }
            if (G.GuessImageUI == this)
            {
                G.GuessImageUI = null;
            }
        }

        public void SetColor(Color color)
        {
            image.color = color;
        }

        void HandleState(RoundState state)
        {
            image.gameObject.SetActive(state != RoundState.Showing);
        }
    }
}
