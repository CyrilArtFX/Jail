using UnityEngine;

using Jail.Environment.Glyphs;

namespace Jail.UI.Glyphs
{
    [RequireComponent(typeof(BoxCollider))]
    public abstract class GlyphBaseUI : GlyphBase
    {
        public bool IsHovered { get; set; }

        const float HOVER_COLOR_OFFSET = 0.5f;

        public virtual void DoClick() {}

        public override void ApplyColor(Color color, int priority = -1)
        {
            if (IsHovered)
            {
                color = new Color(color.r + HOVER_COLOR_OFFSET, color.g + HOVER_COLOR_OFFSET, color.b + HOVER_COLOR_OFFSET, color.a);
            }

            SetColor(color);
        }

        public virtual void SetColor(Color color) {}
    }
}