using UnityEngine;
using UnityEngine.UI;

namespace Mimic.Scripts.UI
{
    // класс отвечает за 3 слайдера
    public class ColorPickerSlidersUI : MonoBehaviour
    {
        [SerializeField] Slider hueSlider;
        [SerializeField] Slider saturationSlider;
        [SerializeField] Slider brightnessSlider;

        [SerializeField] Image huePreview;
        [SerializeField] Image saturationPreview;
        [SerializeField] Image brightnessPreview;

        public Color CurrentColor { get; private set; }

        void Awake()
        {
            G.ColorPickerSlidersUI = this;
        }

        void Start()
        {
            hueSlider.onValueChanged.AddListener(OnSliderChanged);
            saturationSlider.onValueChanged.AddListener(OnSliderChanged);
            brightnessSlider.onValueChanged.AddListener(OnSliderChanged);

            UpdateColor();

            if (G.RoundController != null)
            {
                G.RoundController.OnRoundStateChanged += HandleState;
            }
        }

        void OnDestroy()
        {
            if (G.ColorPickerSlidersUI == this)
            {
                G.ColorPickerSlidersUI = null;
            }
            if (G.RoundController != null)
            {
                G.RoundController.OnRoundStateChanged -= HandleState;
            }
        }

        public void SetInteractable(bool value)
        {
            hueSlider.interactable = value;
            saturationSlider.interactable = value;
            brightnessSlider.interactable = value;
        }

        public void ResetPicker()
        {
            hueSlider.value = 0f;
            saturationSlider.value = 1f;
            brightnessSlider.value = 1f;

            UpdateColor();
        }

        void OnSliderChanged(float _)
        {
            UpdateColor();
        }

        void UpdateColor()
        {
            CurrentColor = Color.HSVToRGB(
                hueSlider.value,
                saturationSlider.value,
                brightnessSlider.value
            );

            G.OctopusUI?.SetColor(CurrentColor);

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

        void HandleState(RoundState state)
        {
            hueSlider.gameObject.SetActive(state == RoundState.Guessing);
            saturationSlider.gameObject.SetActive(state == RoundState.Guessing);
            brightnessSlider.gameObject.SetActive(state == RoundState.Guessing);
        }
    }
}
