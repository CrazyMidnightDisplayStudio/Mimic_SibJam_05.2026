using System.Collections;
using TMPro;
using UnityEngine;

namespace Mimic.Scripts.UI
{
    public class HexResultUI : MonoBehaviour
    {
        [SerializeField] TMP_Text targetHexText;
        [SerializeField] TMP_Text guessHexText;

        [SerializeField] float charDelay = 0.04f;

        Coroutine _animation;

        void Start()
        {
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
            if (state == RoundState.Calculating)
            {
                ShowHexAnim();
            }
            else
            {
                Clear();
            }
        }

        void ShowHexAnim()
        {
            if (_animation != null)
            {
                StopCoroutine(_animation);
            }

            var targetColor = G.GameManager.TargetColor;
            var guessedColor = G.ColorPickerUI.CurrentColor;

            string targetHex = Utils.ColorToHex(targetColor);
            string guessHex = Utils.ColorToHex(guessedColor);

            _animation = StartCoroutine(AnimateHex(targetHex, guessHex));
        }

        IEnumerator AnimateHex(string targetHex, string guessHex)
        {
            targetHexText.SetText("Target: ");
            guessHexText.SetText("Your:   ");

            yield return AnimateText(targetHexText, "Target color: ", targetHex);
            yield return new WaitForSeconds(0.15f);
            yield return AnimateText(guessHexText, "Your color:   ", guessHex);

            _animation = null;
        }

        IEnumerator AnimateText(TMP_Text text, string prefix, string value)
        {
            for (int i = 0; i <= value.Length; i++)
            {
                text.SetText(prefix + value.Substring(0, i));
                yield return new WaitForSeconds(charDelay);
            }
        }

        void Clear()
        {
            if (_animation != null)
            {
                StopCoroutine(_animation);
                _animation = null;
            }

            targetHexText.SetText("");
            guessHexText.SetText("");
        }
    }
}
