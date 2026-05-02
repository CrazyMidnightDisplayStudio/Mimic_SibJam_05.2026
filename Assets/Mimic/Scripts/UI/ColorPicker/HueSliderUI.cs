using UnityEngine;
using UnityEngine.UI;

namespace Mimic.Scripts.UI.ColorPicker
{
    [RequireComponent(typeof(Slider))]
    public class HueSliderUI : MonoBehaviour
    {
        [SerializeField] Image background;

        Slider slider;
        ColorPickerUI colorPicker;

        void Awake()
        {
            slider = GetComponent<Slider>();

            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.wholeNumbers = false;

            background.sprite = CreateHueSprite();
        }

        void Start()
        {
            colorPicker = G.ColorPickerUI;

            slider.onValueChanged.AddListener(OnSliderChanged);
            colorPicker.OnHueChanged += HandleHueChanged;

            slider.SetValueWithoutNotify(colorPicker.Hue);
        }

        void OnDestroy()
        {
            slider.onValueChanged.RemoveListener(OnSliderChanged);

            if (colorPicker != null)
                colorPicker.OnHueChanged -= HandleHueChanged;
        }

        void OnSliderChanged(float value)
        {
            colorPicker.SetHue(value);
        }

        void HandleHueChanged(float value)
        {
            slider.SetValueWithoutNotify(value);
        }

        Sprite CreateHueSprite()
        {
            const int width = 16;
            const int height = 256;

            Texture2D tex = new Texture2D(width, height);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;

            for (int y = 0; y < height; y++)
            {
                float h = y / (float)(height - 1);
                Color color = Color.HSVToRGB(h, 1f, 1f);

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
