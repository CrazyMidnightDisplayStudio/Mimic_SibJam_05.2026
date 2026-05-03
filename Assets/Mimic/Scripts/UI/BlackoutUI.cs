using UnityEngine;
namespace Mimic.Scripts.UI
{
    public class BlackoutUI : MonoBehaviour
    {
        [SerializeField] GameObject blackoutRoot;

        void Start()
        {
            if (G.RoundController)
            {
                G.RoundController.OnRoundStateChanged += HandleRoundStateChanged;
            }
        }

        void HandleRoundStateChanged(RoundState state)
        {
            blackoutRoot.SetActive(state == RoundState.Initialization);
        }
    }
}
