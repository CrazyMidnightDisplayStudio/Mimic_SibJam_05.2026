using UnityEngine;
using UnityEngine.UI;

namespace Mimic.Scripts.UI
{
    public class ColorPickerUI : MonoBehaviour
    {
        [SerializeField] Slider hueSlider;
        [SerializeField] Slider saturationSlider;
        [SerializeField] Slider brightnessSlider;

        [SerializeField] Image guessPreview;

        [SerializeField] Image huePreview;
        [SerializeField] Image saturationPreview;
        [SerializeField] Image brightnessPreview;

        public Color CurrentColor { get; private set; }

        void Awake()
        {
            G.ColorPickerUI = this;
        }
        void OnDestroy()
        {
            if (G.ColorPickerUI == this)
            {
                G.ColorPickerUI = null;
            }
        }

        void Start()
        {
            hueSlider.onValueChanged.AddListener(_ => UpdateColor());
            saturationSlider.onValueChanged.AddListener(_ => UpdateColor());
            brightnessSlider.onValueChanged.AddListener(_ => UpdateColor());
        }

        public void SetInteractable(bool value)
        {
            hueSlider.interactable = value;
            saturationSlider.interactable = value;
            brightnessSlider.interactable = value;
        }

        public void ResetPicker()
        {
            hueSlider.value = 0;
            saturationSlider.value = 0;
            brightnessSlider.value = 0;

            UpdateColor();
        }

        void UpdateColor()
        {
            CurrentColor = Color.HSVToRGB(
                hueSlider.value,
                saturationSlider.value,
                brightnessSlider.value
            );

            guessPreview.color = CurrentColor;

            huePreview.color = Color.HSVToRGB(hueSlider.value, 1f, 1f);
            saturationPreview.color = Color.HSVToRGB(
                hueSlider.value,
                saturationSlider.value,
                1f
            );
            brightnessPreview.color = Color.HSVToRGB(
                hueSlider.value,
                1f,
                brightnessSlider.value
            );
        }
    }
}
