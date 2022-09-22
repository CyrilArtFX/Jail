using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Jail.Environment.Glyphs
{
    public class GlyphTMPButton : GlyphTMP
    {
        public UnityEvent OnClickEvent;

        public bool IsHovered { get; set; }

        const float HOVER_COLOR_OFFSET = 0.5f;

        public void DoClick()
        {
            if (OnClickEvent == null) return;
         
            OnClickEvent.Invoke();
        }

        public override void ApplyColor(Color color, int priority = -1)
        {
            if (IsHovered)
            {
                color = new Color(color.r - HOVER_COLOR_OFFSET, color.g - HOVER_COLOR_OFFSET, color.b - HOVER_COLOR_OFFSET, color.a);
            }

            base.ApplyColor(color, priority);
        }
    }
}