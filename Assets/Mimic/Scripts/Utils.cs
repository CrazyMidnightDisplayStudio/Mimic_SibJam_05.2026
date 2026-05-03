using UnityEngine;

namespace Mimic.Scripts
{
    public static class Utils
    {
        static readonly float HueWeight = 2.0f;
        static readonly float SaturationWeight = 0.8f;
        static readonly float BrightnessWeight = 0.8f;

        static readonly float StrictHueWeight = 4.5f;
        static readonly float StrictSaturationWeight = 1.1f;
        static readonly float StrictBrightnessWeight = 1.0f;

        static readonly string[] Adjectives =
        {
            "Прекрасная",
            "Быстрая",
            "Смелая",
            "Яркая",
            "Ловкая",
            "Милая",
            "Сильная",
            "Добрая",
            "Чудесная",
            "Гордая",
            "Нежная",
            "Умная",
            "Изящная",
            "Веселая",
            "Шустрая",
            "Славная",
            "Отважная",
            "Грациозная",
            "Очаровательная",
            "Великолепная"
        };

        static readonly string[] SeaCreatures =
        {
            "Медуза",
            "Акула",
            "Креветка",
            "Русалка",
            "Касатка",
            "Сардина",
            "Камбала",
            "Черепаха",
            "Мурена",
            "Устрица",
            "Мидия",
            "Ракушка",
            "Ставрида",
            "Каракатица",
            "Селедка",
            "МорскаяЗвезда",
            "Крабиха",
            "Сайра",
            "Рыбка",
            "Пиранья"
        };

        public static float CalculateScoreOld(Color target, Color guess)
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

        public static float CalculateScore(Color target, Color guess)
        {
            Color.RGBToHSV(target, out float targetH, out float targetS, out float targetV);
            Color.RGBToHSV(guess, out float guessH, out float guessS, out float guessV);

            float hueDiff = Mathf.Abs(targetH - guessH);
            hueDiff = Mathf.Min(hueDiff, 1f - hueDiff);

            float saturationDiff = Mathf.Abs(targetS - guessS);
            float brightnessDiff = Mathf.Abs(targetV - guessV);

            float avgSaturation = (targetS + guessS) * 0.5f;
            float hueImportance = Mathf.Lerp(0.35f, 1f, Mathf.Pow(avgSaturation, 0.7f));

            float weightedHueDiff = Mathf.Pow(hueDiff * 2f, 1.8f);
            float weightedSaturationDiff = Mathf.Pow(saturationDiff, 1.35f);
            float weightedBrightnessDiff = Mathf.Pow(brightnessDiff, 1.25f);

            float diff =
                weightedHueDiff * StrictHueWeight * hueImportance +
                weightedSaturationDiff * StrictSaturationWeight +
                weightedBrightnessDiff * StrictBrightnessWeight;

            float maxDiff =
                StrictHueWeight * hueImportance +
                StrictSaturationWeight +
                StrictBrightnessWeight;

            float similarity = 1f - diff / maxDiff;

            float huePenalty = Mathf.Pow(hueDiff * 2f, 1.15f) * 0.75f * hueImportance;
            float maxChannelDiff = Mathf.Max(hueDiff * 2f, saturationDiff, brightnessDiff);
            float mismatchPenalty = Mathf.Pow(maxChannelDiff, 1.4f) * 0.35f;

            similarity -= huePenalty;
            similarity -= mismatchPenalty;

            return Mathf.Clamp01(similarity) * 100f;
        }

        public static string GetRandomName()
        {
            string adjective = Adjectives[Random.Range(0, Adjectives.Length)];
            string seaCreature = SeaCreatures[Random.Range(0, SeaCreatures.Length)];
            int number = Random.Range(10, 100);

            return $"{adjective}_{seaCreature}_{number}";
        }

        public static string ColorToHex(Color color)
        {
            Color32 c = color;
            return $"#{c.r:X2}{c.g:X2}{c.b:X2}";
        }
    }
}
