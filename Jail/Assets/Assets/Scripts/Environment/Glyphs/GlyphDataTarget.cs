using System;
using UnityEngine;

using Jail.Interactables;

namespace Jail.Environment.Glyphs
{
    [Serializable]
    public class GlyphDataTarget
    {
        public GlyphTarget target;
        public float distance = 8.0f;
        public Gradient gradient;
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

                Debug.Log("retrieve transform for " + target);
            }

            return transform;
        }

        public float GetDistToSqr()
        {
            //  compute squared distance
            if (distToSqr == -1.0f)
            {
                distToSqr = distance * distance;
                Debug.Log("compute squared distance ");
            }

            return distToSqr;
        }
    }
}