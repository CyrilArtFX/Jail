using System.Collections;
using UnityEngine;

using Jail.Environment.Glyphs;
using System.Collections.Generic;

namespace Jail.UI
{
	public class CreditMenuController : MenuController
	{
		GlyphBase[] glyphs;

		void Start()
		{
			glyphs = GetComponentsInChildren<GlyphBase>();

			UnSelect();
		}

		public override void Select()
		{
			base.Select();

			foreach (GlyphBase glyph in glyphs)
			{
				glyph.AlphaMultiplier = 1.0f;
			}
		}

		public override void UnSelect()
		{
			base.UnSelect();

			foreach (GlyphBase glyph in glyphs)
			{
				glyph.AlphaMultiplier = 0.0f;
			}
		}
	}
}