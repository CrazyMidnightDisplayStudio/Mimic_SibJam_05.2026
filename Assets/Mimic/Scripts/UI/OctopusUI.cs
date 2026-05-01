using UnityEngine;

namespace Mimic.Scripts.UI
{
    public class OctopusUI : MonoBehaviour
    {
        [SerializeField] GameObject uiRoot;

        [SerializeField] SpriteRenderer mainImage;
        [SerializeField] SpriteRenderer shadowsImage;
        [SerializeField] SpriteRenderer highlightsImage;

        void Awake()
        {
            G.OctopusUI = this;
        }

        void Start()
        {
            G.RoundController.OnRoundStateChanged += HandleRoundStateChanged;
            uiRoot.SetActive(false);
        }
        void OnDestroy()
        {
            if (G.RoundController != null)
            {
                G.RoundController.OnRoundStateChanged -= HandleRoundStateChanged;
            }
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
            shadowsImage.color = Color.HSVToRGB(h, shadowS, shadowV);
        }

        private void SetHighlightsColor(Color color)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);

            // Чем темнее основной цвет — тем слабее блик
            float brightnessFactor = Mathf.Lerp(0.3f, 1f, v);

            float lightS = s * 0.2f; // почти белый
            float lightV = Mathf.Lerp(v, 1f, brightnessFactor);

            highlightsImage.color = Color.HSVToRGB(h, lightS, lightV);
        }

        private void HandleRoundStateChanged(RoundState state)
        {
            uiRoot.SetActive(state != RoundState.None);
        }
    }
}
