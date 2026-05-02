using UnityEngine;

namespace Mimic.Scripts
{
    public static class Utils
    {
        static readonly float HueWeight = 2.0f;
        static readonly float SaturationWeight = 0.8f;
        static readonly float BrightnessWeight = 0.8f;

        public static float CalculateScore(Color target, Color guess)
        {
            Color.RGBToHSV(target, out float targetH, out float targetS, out float targetV);
            Color.RGBToHSV(guess, out float guessH, out float guessS, out float guessV);

            float hueDiff = Mathf.Abs(targetH - guessH);
            hueDiff = Mathf.Min(hueDiff, 1f - hueDiff);

            float saturationDiff = Mathf.Abs(targetS - guessS);
            float brightnessDiff = Mathf.Abs(targetV - guessV);

            float avgSaturation = (targetS + guessS) * 0.5f;
            float hueWeight = HueWeight * avgSaturation;

            float diff =
                hueDiff * hueWeight +
                saturationDiff * SaturationWeight +
                brightnessDiff * BrightnessWeight;

            float maxDiff = hueWeight * 0.5f + SaturationWeight + BrightnessWeight;

            float similarity = 1f - diff / maxDiff;

            float maxChannelDiff = Mathf.Max(
                hueDiff * 2f,
                saturationDiff,
                brightnessDiff
            );

            float penalty = Mathf.Pow(maxChannelDiff, 1.2f);

            similarity -= penalty * 0.45f;

            return Mathf.Clamp01(similarity) * 100f;
        }
    }
}
