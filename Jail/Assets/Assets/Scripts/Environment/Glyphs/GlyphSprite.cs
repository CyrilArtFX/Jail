using UnityEngine;

namespace Jail.Environment.Glyphs
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class GlyphSprite : GlyphBase
    {
        new SpriteRenderer renderer;

        private void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();          
        }

        public override void ApplyColor(Color color)
        {
            renderer.color = color;
        }
    }
}