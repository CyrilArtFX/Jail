using UnityEngine;
using System.Collections;

namespace Jail.Puzzler.Inputs
{
	public class PuzzleLeverInput : PuzzleBaseInput
	{
		bool isPlayerInFront = false;

		void Update()
		{
			if ( !isPlayerInFront ) return;

			if ( Input.GetButtonDown( "Interact" ) )
            {
				IsTriggered = !IsTriggered;
            }
		}

		void OnTriggerEnter( Collider other )
		{
			isPlayerInFront = true;
		}

		void OnTriggerExit( Collider other )
		{
			isPlayerInFront = false;
		}
	}
}
