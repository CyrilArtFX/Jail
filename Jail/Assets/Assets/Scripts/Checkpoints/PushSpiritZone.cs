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

        [SerializeField]
        bool reversed = false;

        void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
        }

        void OnTriggerStay(Collider other)
        {
            if (!LayerMaskUtils.HasLayer(detectedLayers, other.gameObject.layer))
                return;

            float spirit_pos_y = other.transform.position.y;
            float box_max_y = reversed ? transform.position.y + boxCollider.center.y - (boxCollider.size.y / 2) : transform.position.y + boxCollider.center.y + (boxCollider.size.y / 2);
            float box_size_y = boxCollider.size.y;

            //  push_value equals 0 if the spirit is on the top of the box collider and equals 1.5 if the spirit is on the bottom
            float push_value = reversed ? (-(spirit_pos_y + box_max_y) / box_size_y * 1.5f) / 300.0f : -(spirit_pos_y - box_max_y) / box_size_y * 1.5f;
            push_value = reversed ? Mathf.Clamp(push_value, -1.5f, 0.0f) : Mathf.Clamp(push_value, 0.0f, 1.5f);

            other.attachedRigidbody.AddForce(Vector3.up * push_value * 40f); //  push the spirit upwards using push_value
        }
    }
}