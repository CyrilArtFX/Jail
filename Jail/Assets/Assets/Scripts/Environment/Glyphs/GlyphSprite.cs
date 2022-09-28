using UnityEngine;

namespace Jail.Environment.Glyphs
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class GlyphSprite : GlyphBase
    {
        new SpriteRenderer renderer;

        protected override void Awake()
        {
            base.Awake();

            renderer = GetComponent<SpriteRenderer>();          
        }

        public override void ApplyColor(Color color, int priority = -1)
        {
            renderer.color = color;
        }
    }
}