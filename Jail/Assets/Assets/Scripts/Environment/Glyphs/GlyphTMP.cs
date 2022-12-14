using System.Collections;
using UnityEngine;

namespace Jail.Environment.Glyphs
{
    [RequireComponent(typeof(TMPro.TextMeshPro))]
    public class GlyphTMP : GlyphBase
    {
        TMPro.TextMeshPro textMesh;

        protected override void Awake()
        {
            base.Awake();

            textMesh = GetComponent<TMPro.TextMeshPro>();    
        }

        public override void ApplyColor(Color color, int priority = -1)
        {
            textMesh.color = color;
        }
    }
}