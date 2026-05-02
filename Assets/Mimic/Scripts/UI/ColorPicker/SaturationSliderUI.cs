using UnityEngine;
using UnityEngine.UI;

namespace Mimic.Scripts.UI.ColorPicker
{
    [RequireComponent(typeof(Slider))]
    public class SaturationSliderUI : MonoBehaviour
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

            colorPicker.OnSaturationChanged += HandleSaturationChanged;
            colorPicker.OnHueChanged += HandleHueChanged;
            colorPicker.OnBrightnessChanged += HandleBrightnessChanged;

            slider.SetValueWithoutNotify(colorPicker.Saturation);
            UpdateBackground();
        }

        void OnDestroy()
        {
            if (colorPicker == null)
                return;

            slider.onValueChanged.RemoveListener(OnSliderChanged);

            colorPicker.OnSaturationChanged -= HandleSaturationChanged;
            colorPicker.OnHueChanged -= HandleHueChanged;
            colorPicker.OnBrightnessChanged -= HandleBrightnessChanged;
        }

        void OnSliderChanged(float value)
        {
            colorPicker.SetSaturation(value);
        }

        void HandleSaturationChanged(float value)
        {
            slider.SetValueWithoutNotify(value);
        }

        void HandleHueChanged(float _)
        {
            UpdateBackground();
        }

        void HandleBrightnessChanged(float _)
        {
            UpdateBackground();
        }

        void UpdateBackground()
        {
            background.sprite = CreateVerticalGradientSprite(
                Color.HSVToRGB(colorPicker.Hue, 0f, colorPicker.Brightness),
                Color.HSVToRGB(colorPicker.Hue, 1f, colorPicker.Brightness)
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
