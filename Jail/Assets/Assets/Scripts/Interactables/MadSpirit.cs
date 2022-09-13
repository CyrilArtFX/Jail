using UnityEngine;

namespace Jail.Interactables
{
	public class MadSpirit : MonoBehaviour
	{
		public static MadSpirit instance;

		void Awake()
		{
			instance = this;
		}
	}
}