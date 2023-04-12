using Microsoft.Maui.Controls.Platform.Compatibility;

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
		ShanedlerSamples.Library.iOSSpecific.Shell.SetPrefersLargeTitles(Shell.Current, flag);
#endif
		SemanticScreenReader.Announce(CounterBtn.Text);
	}
}

