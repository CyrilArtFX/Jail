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
        public List<Transform> Items => items;

        [SerializeField]
        BezierSpline spline;
        [SerializeField]
        GameObject prefab;
        [SerializeField]
        float scale = .2f;

        [SerializeField]
        float stepSize = .25f;

        [SerializeField, Range(0.0f, 1.0f)]
        float ratio = 1.0f;

        List<Transform> items = new List<Transform>();
        bool toUpdate = false;

        public void RemoveItemsAtRange(int from_id, int to_id)
        {
            //  destroy items
            for (int j = from_id; j <= to_id; j++)
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

            //  remove registered items
            items.RemoveRange(from_id, items.Count - from_id);
        }

        public void DoUpdate()
        {
            if (!spline)
            {
                Debug.LogError("BezierSplineChainer: Spline wasn't found!");
                return;
            }
            if (!prefab) return;
            if (scale <= 0.0f || stepSize <= 0.0f) return;

            //  place items
            int i = 0, created_items_count = 0;
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
                    created_items_count++;
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
            RemoveItemsAtRange(i, items.Count - 1);
        }

        public void QueueUpdate()
        {
            toUpdate = true;
        }

        void SetupSpline()
        {
            //  try to retrieve spline automatically
            TryGetComponent(out spline);

            //  compute length
            spline.ComputeLength();

            //  update when spline is edited
            spline.OnSplineEdited.AddListener(QueueUpdate);
        }

        public void Reset()
        {
            SetupSpline();
            RemoveItemsAtRange(0, Items.Count - 1);
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