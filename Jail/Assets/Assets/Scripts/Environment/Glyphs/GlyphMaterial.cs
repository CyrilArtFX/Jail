using UnityEngine;

namespace Jail.Environment.Glyphs
{
    [RequireComponent(typeof(MeshRenderer))]
    public class GlyphMaterial : GlyphBase
    {
        new MeshRenderer renderer;

        void Awake()
        {
            renderer = GetComponent<MeshRenderer>();
        }

        public override void ApplyColor(Color color)
        {
            renderer.material.color = color;
        }
    }
}