using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Jail.Utility.Bezier
{
    [ExecuteInEditMode]
    public class BezierSplineChainer : MonoBehaviour
    {
        public float Ratio { get => ratio; set => ratio = value; }
        public BezierSpline Spline => spline;

        [SerializeField]
        BezierSpline spline;
        [SerializeField]
        GameObject prefab;
        [SerializeField]
        float scale = .2f;

        [SerializeField]
        float stepSize = .5f;

        [SerializeField, Range(0.0f, 1.0f)]
        float ratio = 1.0f;

        List<Transform> items = new List<Transform>();
        bool toUpdate = false;

        public void DoUpdate()
        {
            if (!spline || !prefab) return;
            if (scale <= 0.0f || stepSize <= 0.0f) return;

            //  place items
            int i = 0;
            float dist = 0.0f;
            while (dist < spline.Length)
            {
                //  compute ratio
                float dist_ratio = dist / spline.Length;
                if (dist_ratio > ratio) break;

                //  get transform item
                Transform item;
                if (items.Count <= i)
                {
                    //  create object
                    item = Instantiate(prefab, transform).transform;

                    //  register item
                    items.Add(item);
                }
                else
                {
                    //  re-use last item
                    item = items[i];
                }

                //  set position
                Vector3 point = spline.GetPoint(dist_ratio);
                item.position = point;

                //  rotate
                item.LookAt(point + spline.GetDirection(dist_ratio));
                if (i % 2 == 0)
                {
                    //  give a chain look
                    item.Rotate(Vector3.forward, 90.0f);
                }

                //  scale
                item.localScale = new Vector3(scale, scale, scale);

                //  increase dist
                dist += stepSize;
                i++;
            }

            //  remove last items
            for (int j = i; j < items.Count; j++)
            {
                Transform item = items[j];
                if (item != null)
                {
                    #if UNITY_EDITOR
                        DestroyImmediate(item.gameObject);
                    #else
                        Destroy(item.gameObject);
                    #endif
                }
            }
            items.RemoveRange(i, items.Count - i);
        }

        public void QueueUpdate()
        {
            toUpdate = true;
        }

        void SetupSpline()
        {
            //  try to retrieve spline automatically
            TryGetComponent(out spline);

            //  update when spline is edited
            spline.OnSplineEdited.AddListener(QueueUpdate);
        }

        void Awake()
        {
            SetupSpline();
            QueueUpdate();
        }

        void OnValidate()
        {
            //  get children as items
            items = transform.Cast<Transform>().ToList();

            SetupSpline();
            QueueUpdate();
        }

        void Update()
        {
            if (toUpdate)
            {
                DoUpdate();
                toUpdate = false;
            }
        }
    }
}