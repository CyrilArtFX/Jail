using UnityEngine;
using System.Collections.Generic;
using Jail.Puzzler.Inputs;

namespace Jail.Puzzler.Outputs
{
	public class PuzzleBaseOutput : MonoBehaviour
	{
		[Header( "Base" ), Tooltip( "List of inputs that need to be turned on in order to trigger this object" ), SerializeField]
		protected List<PuzzleBaseInput> inputs;

		public bool IsTriggered { get; protected set; }

		protected virtual void Awake()
		{
			//  link all inputs to this output
			foreach ( PuzzleBaseInput input in inputs ) 
			{
				input.LinkOutput( this );
			}
		}

		public void AlertInputStateChange( PuzzleBaseInput input, bool state )
		{
			OnInputStateChanged( input, state );

			//  check for all inputs state
			if ( state )
			{
				if ( AreInputsTriggered() )
				{
					IsTriggered = true;
					OnInputsTriggered();
				}
			}
			//  untrigger if one of the inputs lose trigger
			else if ( IsTriggered )
			{
				IsTriggered = false;
				OnInputsUnTriggered();
			}
		}

		public virtual void OnInputStateChanged( PuzzleBaseInput input, bool state )
		{
			print( input + " has changed to state " + state );
		}

		public virtual void OnInputsTriggered()
		{
			print( "all inputs are triggered!" );
		}

		public virtual void OnInputsUnTriggered()
		{
			print( "one of the inputs is untriggered" );
		}

		public bool AreInputsTriggered()
		{
			foreach ( PuzzleBaseInput input in inputs )
			{
				if ( !input.IsTriggered )
				{
					return false;
				}
			}

			return true;
		}
	}
}
