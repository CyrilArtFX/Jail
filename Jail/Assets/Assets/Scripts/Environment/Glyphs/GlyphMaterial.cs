using UnityEngine;

namespace Jail.Environment.Glyphs
{
    [RequireComponent(typeof(MeshRenderer))]
    public class GlyphMaterial : GlyphBase
    {
        new MeshRenderer renderer;

        protected override void Awake()
        {
            base.Awake();

            renderer = GetComponent<MeshRenderer>();
        }

        public override void ApplyColor(Color color, int priority = -1)
        {
            renderer.material.color = color;
        }
    }
}