using UnityEngine;
using UnityEngine.UI;

namespace Mimic.Scripts
{
    public class TargetImageUI : MonoBehaviour
    {
        [SerializeField] Image image;

        void Awake()
        {
            image.gameObject.SetActive(false);
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
            image.gameObject.SetActive(state == RoundState.Showing);
        }
    }
}
