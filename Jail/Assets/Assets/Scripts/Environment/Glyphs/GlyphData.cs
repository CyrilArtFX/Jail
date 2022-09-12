using UnityEngine;

namespace Jail.Environment.Glyphs
{
    [CreateAssetMenu(fileName = "New Glyph Data", menuName = "ScriptableObjects/GlyphData", order = 1)]
    public class GlyphData : ScriptableObject
    {
        [Header("Coloring")]
        public float distance = 8.0f;
        public Gradient gradient;
        public float smoothFactor = 5.0f;

        [Header("RGB Gamer Mode")]
        public bool isRGBGamer = false;
        public bool isDistanceDependent = false;
    }
}