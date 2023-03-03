namespace ShanedlerSamples;

public partial class MainTabbedPage
{
	public MainTabbedPage()
	{
		InitializeComponent();
	}

	private void Button_Clicked(object sender, EventArgs e)
	{
		this.Children.Add(new MainPage() { Title = "new page" });
	}
}