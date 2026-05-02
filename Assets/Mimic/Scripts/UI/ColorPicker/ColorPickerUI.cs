using System;
using UnityEngine;

namespace Mimic.Scripts.UI.ColorPicker
{
    public class ColorPickerUI : MonoBehaviour
    {
        [SerializeField] string startColorHex = "#B550FA";
        [SerializeField] GameObject slidersRoot;

        public float Hue { get; private set; }
        public float Saturation { get; private set; }
        public float Brightness { get; private set; }

        public Color CurrentColor { get; private set; }

        public bool Interactable { get; private set; }

        public event Action<float> OnHueChanged;
        public event Action<float> OnSaturationChanged;
        public event Action<float> OnBrightnessChanged;
        public event Action<Color> OnColorChanged;
        public event Action<bool> OnInteractableChanged;


        void Awake()
        {
            G.ColorPickerUI = this;

            if (!ColorUtility.TryParseHtmlString(startColorHex, out Color startColor))
                startColor = Color.white;

            SetColor(startColor, notify: false);
        }

        void Start()
        {
            NotifyAll();
        }

        void OnDestroy()
        {
            if (G.ColorPickerUI == this)
                G.ColorPickerUI = null;
        }

        public void SetInteractable(bool value)
        {
            if (Interactable == value)
                return;

            Interactable = value;
            slidersRoot.SetActive(value);
            OnInteractableChanged?.Invoke(value);
        }

        public void SetHue(float hue)
        {
            hue = Mathf.Clamp01(hue);
            if (Mathf.Approximately(Hue, hue))
                return;

            Hue = hue;
            UpdateCurrentColor();

            OnHueChanged?.Invoke(Hue);
            OnColorChanged?.Invoke(CurrentColor);
        }

        public void SetSaturation(float saturation)
        {
            saturation = Mathf.Clamp01(saturation);
            if (Mathf.Approximately(Saturation, saturation))
                return;

            Saturation = saturation;
            UpdateCurrentColor();

            OnSaturationChanged?.Invoke(Saturation);
            OnColorChanged?.Invoke(CurrentColor);
        }

        public void SetBrightness(float brightness)
        {
            brightness = Mathf.Clamp01(brightness);
            if (Mathf.Approximately(Brightness, brightness))
                return;

            Brightness = brightness;
            UpdateCurrentColor();

            OnBrightnessChanged?.Invoke(Brightness);
            OnColorChanged?.Invoke(CurrentColor);
        }

        public void SetColor(Color color, bool notify = true)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);

            Hue = h;
            Saturation = s;
            Brightness = v;

            UpdateCurrentColor();

            if (notify)
                NotifyAll();
        }

        public void ResetPicker()
        {
            if (!ColorUtility.TryParseHtmlString(startColorHex, out Color startColor))
                startColor = Color.white;

            SetColor(startColor);
        }

        void UpdateCurrentColor()
        {
            CurrentColor = Color.HSVToRGB(Hue, Saturation, Brightness);
        }

        void NotifyAll()
        {
            OnHueChanged?.Invoke(Hue);
            OnSaturationChanged?.Invoke(Saturation);
            OnBrightnessChanged?.Invoke(Brightness);
            OnColorChanged?.Invoke(CurrentColor);
        }
    }
}
