
namespace Jail.Puzzler.Outputs
{
	public class PuzzleDoorOutput : PuzzleBaseOutput
	{
		AutomaticSlider slider;

		protected override void Awake()
		{
			base.Awake();

			slider = GetComponent<AutomaticSlider>();
			slider.enabled = false;
		}

		public override void OnInputsTriggered()
		{
			print( "Opening door!" );

			slider.enabled = true;
			slider.Reversed = false;
		}

		public override void OnInputsUnTriggered()
		{
			print( "Closing door!" );

			slider.enabled = true;
			slider.Reversed = true;
		}
	}
}
