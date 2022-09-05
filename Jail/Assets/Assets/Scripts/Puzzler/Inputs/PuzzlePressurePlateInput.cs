using UnityEngine;
using System.Collections;

namespace Jail.Puzzler.Inputs
{
	public class PuzzlePressurePlateInput : PuzzleBaseInput
	{
		[Header( "Pressure Plate" ), Tooltip( "How much time should it wait before turning the trigger off after exit?" ), SerializeField]
		float exitTriggerTime = 0.0f;

		Coroutine oldExitCoroutine;

		void OnTriggerEnter( Collider other )
		{
			IsTriggered = true;

			if ( !( oldExitCoroutine == null ) )
				StopCoroutine( oldExitCoroutine );
		}

		void OnTriggerExit( Collider other )
		{
			oldExitCoroutine = StartCoroutine( CoroutineExitTrigger() );
		}

		IEnumerator CoroutineExitTrigger()
		{
			yield return new WaitForSeconds( exitTriggerTime );

			IsTriggered = false;
		}
	}
}
