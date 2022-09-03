﻿using UnityEngine;
using System.Collections.Generic;
using Jail.Puzzler.Outputs;

namespace Jail.Puzzler.Inputs
{
	public class PuzzleBaseInput : MonoBehaviour
	{
		protected TwoStatesAnim twoStatesAnim;

		protected virtual void Awake()
		{
			TryGetComponent( out twoStatesAnim );
		}

		//  output linking
		List<PuzzleBaseOutput> outputs = new List<PuzzleBaseOutput>();
		public void LinkOutput( PuzzleBaseOutput output ) { outputs.Add( output ); }

		//  trigger state
		bool isTriggered = false;
		public bool IsTriggered { 
			get => isTriggered;
			protected set {
				isTriggered = value;
				OnTrigger( value );
			}
		}

		protected virtual void OnTrigger( bool state )
		{
			print( "input: " + this + " state is " + isTriggered );

			//  alert output from change
			foreach ( PuzzleBaseOutput output in outputs )
			{
				output.AlertInputStateChange( this, IsTriggered );
			}

			//  trigger anim
			if ( twoStatesAnim )
				twoStatesAnim.ChangeBool( state );
		} 
	}
}
