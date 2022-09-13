using UnityEngine;

namespace Jail.Environment.Glyphs
{
    [CreateAssetMenu(fileName = "New Glyph Data", menuName = "ScriptableObjects/GlyphData", order = 1)]
    public class GlyphData : ScriptableObject
    {
        [Header("Coloring"), Tooltip("Color gradient used when no target color is used")]
        public Gradient gradient;
        [Tooltip("Color Interpolation speed factor")]
        public float smoothSpeed = 5.0f;
        [Tooltip("Should the current target's color override the gradient defined here?")]
        public bool useTargetColor = true;
        [Tooltip("List of target relations with this glyph type")]
        public GlyphDataTarget[] targets;

        [Header("RGB Gamer Mode"), Tooltip("Only for G@M3RS.")]
        public bool isRGBGamer = false;
        public bool isDistanceDependent = false;
    }
}