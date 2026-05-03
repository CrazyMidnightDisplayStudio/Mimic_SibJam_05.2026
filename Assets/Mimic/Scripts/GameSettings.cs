using UnityEngine;

namespace Mimic.Scripts
{
    [CreateAssetMenu(menuName = "Mimic/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Timing")]
        public float showTime = 5f;
        public float guessTime = 10f;

        [Header("Color Generation")]
        [Header("Saturation")]
        [Range(0f, 1f)] public float minSaturation = 0.7f;
        [Range(0f, 1f)] public float maxSaturation = 1f;

        [Header("Brightness")]
        [Range(0f, 1f)] public float minBrightness = 0.7f;
        [Range(0f, 1f)] public float maxBrightness = 1f;

        [Header("Scoring")]
        public float successThreshold = 0.85f;

        public Color GenerateTargetColor()
        {
            return Random.ColorHSV(
                0f, 1f,
                minSaturation, maxSaturation,
                minBrightness, maxBrightness
            );
        }
    }
}
