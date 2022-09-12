using System;
using UnityEngine;

using Jail.Interactables;

namespace Jail.Environment.Glyphs
{
    [Serializable]
    public class GlyphDataTarget
    {
		[Tooltip("Which character should it links to?")]
        public GlyphTarget target;
		[Tooltip("Target Radius action-effect")]
        public float distance = 8.0f;
		[Tooltip("Color gradient used in the action-ranged glyph")]
        public Gradient gradient;
		[Tooltip("How should we prioritize this target depending on other? Lower value means higher priority")]
        public int priority = 0;

        Transform transform;
        float distToSqr = -1.0f;

        public bool IsActive()
        {
            switch (target)
            {
                case GlyphTarget.PlayerSpirit:
                    return Player.instance.IsSpirit || Player.instance.IsSpiritReturning;
                default:
                    return true;
            }
        }

        public Transform GetTransform()
        {
            //  retrieve transform
            if (transform == null)
            {
                switch (target)
                {
                    case GlyphTarget.PlayerSpirit:
                        transform = Player.instance.Spirit.transform;
                        break;
                    case GlyphTarget.PlayerBody:
                        transform = Player.instance.transform;
                        break;
                    case GlyphTarget.MadSpirit:
                        transform = MadSpirit.instance.transform;
                        break;
                }
            }

            return transform;
        }

        public float GetDistToSqr()
        {
            //  compute squared distance
            if (distToSqr == -1.0f)
            {
                distToSqr = distance * distance;
            }

            return distToSqr;
        }
    }
}