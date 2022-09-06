using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jail.Interactables.ZapTurret
{
    public class ZapTurretChainLink : MonoBehaviour
    {
        public Transform NextAnchor => nextAnchor;
        public HingeJoint Joint { get; protected set; }
        public Rigidbody Rigidbody { get; protected set; }

		[Header("Chain Link"), SerializeField]
        Transform nextAnchor;

        void Awake()
		{
            Joint = GetComponent<HingeJoint>();
            Rigidbody = GetComponent<Rigidbody>();
		}
    }
}
