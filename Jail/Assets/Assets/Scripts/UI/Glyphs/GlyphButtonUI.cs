using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Jail.UI.Glyphs
{
    public class GlyphButtonUI : GlyphBaseUI
    {
        public UnityEvent OnClickEvent;

        TMPro.TextMeshPro textMesh;

        void Awake()
		{
            textMesh = GetComponent<TMPro.TextMeshPro>();
		}

        public override void DoClick()
        {
            if (OnClickEvent == null) return;
         
            OnClickEvent.Invoke();
        }

        public override void SetColor(Color color)
        {
            textMesh.color = color;
        }
    }
}