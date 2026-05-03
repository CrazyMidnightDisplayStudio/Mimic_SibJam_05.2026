using UnityEngine;

namespace Mimic.Scripts.UI
{
    public class OctopusUI : MonoBehaviour
    {
        [SerializeField] float fallingShadowAlpha = 0.5f;
        [SerializeField] GameObject uiRoot;

        [SerializeField] SpriteRenderer mainImage;
        [SerializeField] SpriteRenderer shadowsImage;
        [SerializeField] SpriteRenderer fallingShadow;
        [SerializeField] SpriteRenderer highlightsImage;
        [SerializeField] SpriteRenderer patternImage;

        void Start()
        {
            G.ColorPickerUI.OnColorChanged += HandleColorChanged;
            G.RoundController.OnRoundStateChanged += HandleRoundStateChanged;
            uiRoot.SetActive(false);
            fallingShadow.gameObject.SetActive(false);
        }
        void OnDestroy()
        {
            if (G.RoundController != null)
            {
                G.RoundController.OnRoundStateChanged -= HandleRoundStateChanged;
            }

            if (G.ColorPickerUI != null)
            {
                G.ColorPickerUI.OnColorChanged -= HandleColorChanged;
            }
        }
        void HandleColorChanged(Color color)
        {
            SetColor(color);
        }

        public void SetColor(Color color)
        {
            mainImage.color = color;
            //TODO
            SetHighlightsColor(color);
            SetShadowColor(color);
        }

        private void SetShadowColor(Color color)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            float shadowS = s * 0.6f;
            float shadowV = v * 0.4f;
            Color shadowColor = Color.HSVToRGB(h, shadowS, shadowV);

            shadowsImage.color = shadowColor;
            Color falling = shadowColor;
            falling.a = fallingShadowAlpha;
            fallingShadow.color = falling;
        }

        private void SetHighlightsColor(Color color)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);

            float lightS = Mathf.Clamp01(s * 0.45f);
            float lightV = Mathf.Clamp01(v + 0.35f);

            highlightsImage.color = Color.HSVToRGB(h, lightS, lightV);
            patternImage.color = Color.HSVToRGB(h, lightS, lightV);
        }

        private void HandleRoundStateChanged(RoundState state)
        {
            uiRoot.SetActive(state != RoundState.Menu && state != RoundState.Initialization);
            fallingShadow.gameObject.SetActive(state != RoundState.Menu && state != RoundState.Initialization);
        }
    }
}
