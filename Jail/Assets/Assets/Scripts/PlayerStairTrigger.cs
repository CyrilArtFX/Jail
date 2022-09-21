using UnityEngine;
using Jail.Utility;

namespace Jail
{
    public class PlayerStairTrigger : MonoBehaviour
    {
        [SerializeField]
        LayerMask stairMask = 0;

        private void OnTriggerEnter(Collider other)
        {
            if (LayerMaskUtils.HasLayer(stairMask, other.gameObject.layer))
            {
                Player.instance.StairContact = true;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (LayerMaskUtils.HasLayer(stairMask, other.gameObject.layer))
            {
                Player.instance.StairContact = true;
            }
        }
    }
}
