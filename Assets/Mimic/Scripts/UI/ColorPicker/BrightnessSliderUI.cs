using UnityEngine;
using UnityEngine.UI;

namespace Mimic.Scripts.UI.ColorPicker
{
    [RequireComponent(typeof(Slider))]
    public class BrightnessSliderUI : MonoBehaviour
    {
        Slider slider;
        [SerializeField] Image background;

        ColorPickerUI colorPicker;

        void Awake()
        {
            slider = GetComponent<Slider>();
        }

        void Start()
        {
            colorPicker = G.ColorPickerUI;

            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.wholeNumbers = false;

            slider.onValueChanged.AddListener(OnSliderChanged);

            colorPicker.OnBrightnessChanged += HandleBrightnessChanged;
            colorPicker.OnHueChanged += HandleHueChanged;
            colorPicker.OnSaturationChanged += HandleSaturationChanged;

            slider.SetValueWithoutNotify(colorPicker.Brightness);
            UpdateBackground();
        }

        void OnDestroy()
        {
            if (colorPicker == null)
                return;

            slider.onValueChanged.RemoveListener(OnSliderChanged);

            colorPicker.OnBrightnessChanged -= HandleBrightnessChanged;
            colorPicker.OnHueChanged -= HandleHueChanged;
            colorPicker.OnSaturationChanged -= HandleSaturationChanged;
        }

        void OnSliderChanged(float value)
        {
            colorPicker.SetBrightness(value);
        }

        void HandleBrightnessChanged(float value)
        {
            slider.SetValueWithoutNotify(value);
        }

        void HandleHueChanged(float _)
        {
            UpdateBackground();
        }

        void HandleSaturationChanged(float _)
        {
            UpdateBackground();
        }

        void UpdateBackground()
        {
            background.sprite = CreateVerticalGradientSprite(
                Color.black,
                Color.HSVToRGB(colorPicker.Hue, colorPicker.Saturation, 1f)
            );
        }

        Sprite CreateVerticalGradientSprite(Color bottom, Color top)
        {
            const int width = 16;
            const int height = 256;

            Texture2D tex = new Texture2D(width, height);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;

            for (int y = 0; y < height; y++)
            {
                float t = y / (float)(height - 1);
                Color color = Color.Lerp(bottom, top, t);

                for (int x = 0; x < width; x++)
                    tex.SetPixel(x, y, color);
            }

            tex.Apply();

            return Sprite.Create(
                tex,
                new Rect(0, 0, width, height),
                new Vector2(0.5f, 0.5f)
            );
        }
    }
}
