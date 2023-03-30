namespace Shandler.Samples;

public class MainFlyoutPage : FlyoutPage
{
	public MainFlyoutPage()
	{
		Detail = new NavigationPage(new MainPage());
		Flyout = new ContentPage()
		{
			Content = new VerticalStackLayout()
			{
				new Label()
				{
					Text = "label"
				}
			},
			Title = "rabbits of mercy"
		};
	}
}