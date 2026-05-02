using UnityEngine;

namespace Mimic.Scripts.UI
{
    public class TargetColorPreviewUI : MonoBehaviour
    {
        [SerializeField] SpriteRenderer mainPreview;
        [SerializeField] SpriteRenderer shadowsPreview;

        void Start()
        {
            G.GameManager.OnTargetColorChanged += SetColor;
        }

        void SetColor(Color color)
        {
            mainPreview.color = color;
            //TODO
            SetShadowColor(color);
        }

        void SetShadowColor(Color color)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            float shadowS = s * 0.6f;
            float shadowV = v * 0.4f;
            shadowsPreview.color = Color.HSVToRGB(h, shadowS, shadowV);
        }
    }
}
