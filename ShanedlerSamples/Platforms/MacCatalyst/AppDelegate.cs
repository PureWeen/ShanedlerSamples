using Foundation;
using UIKit;

namespace ShanedlerSamples;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();



	public override void PressesBegan(NSSet<UIPress> presses, UIPressesEvent evt)
	{
		base.PressesBegan(presses, evt);
	}

	public override void PressesEnded(NSSet<UIPress> presses, UIPressesEvent evt)
	{
		base.PressesEnded(presses, evt);
	}

	public override UIKeyCommand[] KeyCommands => base.KeyCommands;
}
