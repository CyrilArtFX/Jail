﻿using System.Collections.Generic;
using UnityEngine;

namespace Jail.Environment.Glyphs
{
    public abstract class GlyphBase : MonoBehaviour
    {
        [SerializeField]
        GlyphData data;

        float t = 0.0f;
        Color smoothPriorityColor;
        Color targetPriorityColor;

        void Start()
		{
            smoothPriorityColor = data.gradient.Evaluate(0.0f);
            targetPriorityColor = smoothPriorityColor;
		}

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

            Color priority_color = data.gradient.Evaluate(0.0f);
            int priority_id = int.MaxValue;
            foreach (GlyphDataTarget target in data.targets)
            {
                //  check for active target
                if (!target.IsActive()) continue;
                //  check for priority
                if (target.priority > priority_id) continue;

                //  get target's transform
                Transform target_transform = target.GetTransform();
                
                //  compute distance ratio
                Vector2 direction = new Vector2(target_transform.position.z - transform.position.z, target_transform.position.y - transform.position.y);  //  flat depth
                float dist_ratio = direction.sqrMagnitude / target.GetDistToSqr();
                if (dist_ratio > 1.0f) continue;

                //  prioritize color
                if (data.useTargetColor)
				{
                    priority_color = target.gradient.Evaluate(1.0f - dist_ratio);
				}
                else
				{
                    priority_color = data.gradient.Evaluate(1.0f - dist_ratio);
				}
                priority_id = target.priority;
            }

            //  listen for color target changes
            if (targetPriorityColor != priority_color)
			{
                //  reset smoothing interpolation on color change
                t = 0.0f;

                //  keep track of targeted color
                targetPriorityColor = priority_color;
			}

            //  increase interpolation value 
            t = Mathf.Clamp01(t + Time.deltaTime * data.smoothFactor);

            //  smoothing color changes
            smoothPriorityColor = Color.Lerp(smoothPriorityColor, targetPriorityColor, t);
            
            //  apply color
            ApplyColor(smoothPriorityColor);
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