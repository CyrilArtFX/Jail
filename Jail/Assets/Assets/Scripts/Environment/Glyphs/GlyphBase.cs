using UnityEngine;

namespace Jail.Environment.Glyphs
{
    public abstract class GlyphBase : MonoBehaviour
    {
        [SerializeField]
        GlyphData data;

        float distToSqr;
        bool hasDetectedTarget = false;
        Transform target;

        float t = 0.0f;

        void Start()
        {
            //  compute distance squared
            distToSqr = data.distance * data.distance;

            //  retrieve target
            target = Player.instance.Spirit.transform;
        }

        void Update()
        {
            //  get direction
            Vector2 direction = new Vector2(target.position.z - transform.position.z, target.position.y - transform.position.y);  //  flat depth
            float dist_ratio = direction.sqrMagnitude / distToSqr;

            //  get gradient evaluation time
            if ((Player.instance.IsSpirit || Player.instance.IsSpiritReturning) && dist_ratio <= 1.0f)
            {
                t = Mathf.MoveTowards(t, 1.0f - dist_ratio, Time.deltaTime * data.smoothFactor);
                hasDetectedTarget = true;
            }
            else
            {
                t = Mathf.MoveTowards(t, 0.0f, Time.deltaTime * data.smoothFactor);
                hasDetectedTarget = false;
            }

            //  evaluate color from gradient
            Color new_color = data.gradient.Evaluate(t);
            if (data.isRGBGamer)
            {
                float hue = (transform.position.z + transform.position.y + Time.time) % 1.0f;
                if (data.isDistanceDependent)
                {
                    hue *= t;
                }
                new_color = Color.HSVToRGB(hue, 1.0f, 1.0f);
            }
            ApplyColor(new_color);
        }

        void OnDrawGizmosSelected()
        {
            if (data == null) return;

            Gizmos.color = hasDetectedTarget ? Color.green : Color.red;

            Vector3 position = transform.position;
            if (target != null)
            {
                position.x = target.position.x;
            }
            Gizmos.DrawWireSphere(position, data.distance);
        }

        public virtual void ApplyColor(Color color) {}
    }
}