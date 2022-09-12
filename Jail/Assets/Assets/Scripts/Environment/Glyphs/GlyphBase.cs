using System.Collections.Generic;
using UnityEngine;

namespace Jail.Environment.Glyphs
{
    public abstract class GlyphBase : MonoBehaviour
    {
        [SerializeField]
        GlyphData data;

        float t = 0.0f;

        void Update()
        {
            //  get direction
            /*Vector2 direction = new Vector2(target.position.z - transform.position.z, target.position.y - transform.position.y);  //  flat depth
            float dist_ratio = direction.sqrMagnitude / distToSqr;

            //  get gradient evaluation time
            if ((Player.instance.IsSpirit || Player.instance.IsSpiritReturning) && dist_ratio <= 1.0f)
            {
                t = Mathf.MoveTowards(t, 1.0f - dist_ratio, Time.deltaTime * data.smoothFactor);
                //hasDetectedTarget = true;
            }
            else
            {
                t = Mathf.MoveTowards(t, 0.0f, Time.deltaTime * data.smoothFactor);
                //hasDetectedTarget = false;
            }

            //  evaluate color from gradient
            Color new_color = data.gradient.Evaluate(t);
            *//*if (data.isRGBGamer)
            {
                float hue = (transform.position.z + transform.position.y + Time.time) % 1.0f;
                if (data.isDistanceDependent)
                {
                    hue *= t;
                }
                new_color = Color.HSVToRGB(hue, 1.0f, 1.0f);
            }*//*
            ApplyColor(new_color);*/

            Color priority_color = data.idleColor;
            int priority_id = int.MaxValue;
            foreach (GlyphDataTarget target in data.targets)
            {
                if (!target.IsActive()) continue;
                if (target.priority > priority_id) continue;

                Transform target_transform = target.GetTransform();

                Vector2 direction = new Vector2(target_transform.position.z - transform.position.z, target_transform.position.y - transform.position.y);  //  flat depth
                
                float dist_ratio = direction.sqrMagnitude / target.GetDistToSqr();
                if (dist_ratio > 1.0f) continue;

                //  prioritize color
                priority_color = data.useTargetColor ? target.color : data.idleColor;
                priority_id = target.priority;
            }

            ApplyColor(priority_color);
        }

        /*void OnDrawGizmosSelected()
        {
            if (data == null) return;

            Gizmos.color = hasDetectedTarget ? Color.green : Color.red;

            Vector3 position = transform.position;
            if (target != null)
            {
                position.x = target.position.x;
            }
            Gizmos.DrawWireSphere(position, data.distance);
        }*/

        public virtual void ApplyColor(Color color) {}
    }
}