using System.Collections.Generic;
using UnityEngine;

namespace Jail.Interactables.ZapTurret
{
	public class ZapTurretChainer : MonoBehaviour
	{
		public GameObject projectileTarget;

		[Header("Chainer"), SerializeField]
		GameObject chainPrefab;
		[SerializeField]
		Transform anchor;

		float currentSpawnTime = 0.0f, spawnTime = 0.5f;
		float linkDistance = 0.0f;
		List<ZapTurretChainLink> chains = new List<ZapTurretChainLink>();
		bool queueForLinking = false;

		new Rigidbody rigidbody;

		void Awake()
		{
			rigidbody = GetComponent<Rigidbody>();
		}

		void FixedUpdate()
		{
			//  check for projectile validity
			if (projectileTarget == null) return;

			//  proceed linking
			if (queueForLinking)
			{
				LinkChainsTogether();
				queueForLinking = false;
			}

			int count = chains.Count;
			//if (count == 3) return;

			//  get farest position from chainer
			Vector3 farestPos = transform.position;
			if (count > 0 )
			{
				ZapTurretChainLink farest_chain = chains[0];
				farestPos = farest_chain.transform.position;
			}

			//  get distance from farest to projectile
			float dist = Vector3.Distance(farestPos, projectileTarget.transform.position);
			if (dist > linkDistance && (currentSpawnTime -= Time.fixedDeltaTime) <= 0.0f)
			{
				currentSpawnTime = spawnTime;
				SpawnChain();
			}
		}

		void SpawnChain()
		{
			int count = chains.Count;

			//  set position
			Transform parent = anchor;
			if (count > 0)
			{
				//  transform to last chain's anchor
				ZapTurretChainLink last_chain = chains[0];
				parent = last_chain.NextAnchor;
			}

			//  spawn chain
			GameObject chain = Instantiate(chainPrefab);
			chain.transform.position = parent.position;

			ZapTurretChainLink script = chain.GetComponent<ZapTurretChainLink>();
			if (count == 0)
			{
				linkDistance = script.Joint.anchor.x;
			}

			//  register to list
			chains.Add(script);

			//  queue for linking
			queueForLinking = true;  //  link next frame so we can add multiple chains this frame w/ only 1 linking
		}

		void LinkChainsTogether()
		{
			int count = chains.Count;
			if (count == 0) return;

			Rigidbody last_anchor = rigidbody;
			for (int i = 0; i < count; i++)
			{
				ZapTurretChainLink chain = chains[i];
				print(i + " " + chain + " " + last_anchor);
				chain.Joint.connectedBody = last_anchor;
				last_anchor = chain.Rigidbody;
			}
		}
	}
}