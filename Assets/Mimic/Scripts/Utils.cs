using UnityEngine;

namespace Mimic.Scripts
{
    public static class Utils
    {
        public static float CalculateScore(Color target, Color guess)
        {
            float distance = Vector3.Distance(
                new Vector3(target.r, target.g, target.b),
                new Vector3(guess.r, guess.g, guess.b)
            );

            float maxDistance = Mathf.Sqrt(3f);

            float similarity = 1f - distance / maxDistance;

            return Mathf.Clamp01(similarity) * 100f;
        }
    }
}
