using System.Collections;
using UnityEngine;

namespace Jail.Environment.Glyphs
{
    public class GlyphTMP : GlyphBase
    {
        TMPro.TextMeshPro textMesh;

        void Awake()
        {
            textMesh = GetComponent<TMPro.TextMeshPro>();    
        }

        public override void ApplyColor(Color color)
        {
            textMesh.color = color;
        }
    }
}