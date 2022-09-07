using System.Collections.Generic;
using UnityEngine;

using Jail.Environment;
using SplineMesh;

namespace Jail.Interactables.ZapTurret
{
	[ExecuteInEditMode]
	public class ZapTurretChainer : MonoBehaviour
	{
		public ZapTurretProjectile Projectile { get; set; }
		public Transform TurretAnchor { get; set; }
		public Spline Spline => splineChain.Spline;
		public SplineChain SplineChain => splineChain;
		public List<SplineNode> Nodes => Spline.nodes;

		[SerializeField]
		SplineChain splineChain;
		[SerializeField]
		float nodeSpawnTime = .2f;

		float currentNodeSpawnTime;
		Dictionary<SplineNode, Vector3> worldNodesPositions = new Dictionary<SplineNode, Vector3>();
		
		/// <summary>
		/// Get farest node from the chainer considering Node 0 is the closest.
		/// </summary>
		/// <returns>Node ID</returns>
		public int RetrieveFarestNode(out SplineNode node) //  TODO: change name
        {
			int id = Nodes.Count - 2;
			node = Nodes[id];
			return id;
        }

		public void RemoveNodeAt(int id)
        {
			SplineNode node = Nodes[id];
			Nodes.Remove(node);
			worldNodesPositions.Remove(node);

			Spline.RefreshCurves();
		}

		void Start()
        {
			//worldNodesPositions.Add(Nodes[1], TurretAnchor.position);  //  pos of last node
			currentNodeSpawnTime = nodeSpawnTime;    
        }

        void FixedUpdate()
		{
			//  check for at least 2 nodes
			int count = Nodes.Count;
			if (count < 2)
			{
				enabled = false;
				Debug.LogError("ZapTurretChainer: There must be only 2 nodes in the Spline!");
				return;
			}

			//  get two first nodes
			SplineNode first_node = Nodes[0], last_node = Nodes[count - 1];

			//  set nodes position
			last_node.Position = transform.InverseTransformPoint(TurretAnchor.position);
			first_node.Position = Vector3.zero;

			//  set nodes direction
			first_node.Direction = (last_node.Position - first_node.Position).normalized;
			last_node.Direction = -first_node.Direction;

			//  update in-between nodes local positions to world positions
			SplineNode previous_node = Nodes[0];
			for (int i = 1; i < count - 1; i++)
            {
				SplineNode node = Nodes[i];
				
				//  update node
				Vector3 world_pos = worldNodesPositions[node];
				node.Position = transform.InverseTransformPoint(world_pos);
				node.Direction = Vector3.zero;// -(previous_node.Position - node.Position).normalized;
				
				previous_node = node;
				//print("update node " + i);
            }

			if (!Projectile.IsReturning)
            {
				//  spawn new node
				if ((currentNodeSpawnTime -= Time.fixedDeltaTime) <= 0.0f)
				{
					//  insert new node
					SplineNode node = new SplineNode(Vector3.zero, Vector3.zero);
					Spline.AddNode(node);
					worldNodesPositions.Add(node, transform.position);

					//for (int i = 0; i < Nodes.Count; i++) { print("before " + i + " " + (Nodes[i] == last_node)); }

					//  sorting last node
					Spline.RemoveNode(last_node);
					Spline.AddNode(last_node);

					Spline.RefreshCurves();
					//for (int i = 0; i < Nodes.Count; i++) { print("after " + i + " " + (Nodes[i] == last_node)); }
					//print(Nodes.IndexOf(last_node) + "  " + (last_node == Nodes[Nodes.Count - 1]));

					//  reset timer
					currentNodeSpawnTime = nodeSpawnTime;
				}
            }
        }

        /*[Header("Chainer"), SerializeField]
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
		}*/
    }
}