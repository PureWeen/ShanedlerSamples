using Microsoft.Maui.Controls.Platform.Compatibility;
using ShanedlerSamples.Library.Common;

namespace ShanedlerSamples;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	bool flag;

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";
#if IOS
		flag = !flag;
        Maui.FixesAndWorkarounds.iOSSpecific.ShellAttachedProperties.SetPrefersLargeTitles(Shell.Current, flag);
#endif
		SemanticScreenReader.Announce(CounterBtn.Text);


		var color = ShellToolbarExtensions.GetToolbarBackgroundColor(this);

		color = Colors.Red == color ? Colors.Blue : Colors.Red;

		ShellToolbarExtensions.SetToolbarBackgroundColor(this, color);
	}
}

