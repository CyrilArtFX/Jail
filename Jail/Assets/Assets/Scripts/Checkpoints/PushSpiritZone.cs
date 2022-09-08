using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jail.Utility;

namespace Jail
{
    public class PushSpiritZone : MonoBehaviour
    {
        [SerializeField]
        LayerMask detectedLayers = -1;

        BoxCollider boxCollider;

        void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
        }

        void OnTriggerStay(Collider other)
        {
            if (!LayerMaskUtils.HasLayer(detectedLayers, other.gameObject.layer))
                return;

            float spirit_pos_y = other.transform.position.y;
            float box_max_y = transform.position.y + boxCollider.center.y + (boxCollider.size.y / 2);
            float box_size_y = boxCollider.size.y;

            float push_value = -(spirit_pos_y - box_max_y) / box_size_y * 1.5f;
            push_value = Mathf.Clamp(push_value, 0f, 1.5f);

            other.attachedRigidbody.AddForce(Vector3.up * push_value * 100f);
        }
    }
}
