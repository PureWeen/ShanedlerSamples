namespace ShanedlerSamples;

public class MainFlyoutPage : FlyoutPage
{
	public MainFlyoutPage()
	{
		Detail = new NavigationPage(new MainPage());
		Flyout = new SwappingStuffFlyoutPage();
		//Flyout = new ContentPage()
		//{
		//	Content = new VerticalStackLayout()
		//	{
		//		new Label()
		//		{
		//			Text = "label"
		//		}
		//	},
		//	Title = "rabbits of mercy"
		//};
	}

	class SwappingStuffFlyoutPage : ContentPage
	{
		public SwappingStuffFlyoutPage()
		{
			Title = "Hi thgere";

			var layout = new VerticalStackLayout();

			layout.Add(new Button()
			{
				Text = "Swap flyout page",
				Command = new Command(() =>
				{
					if (Application.Current.MainPage is FlyoutPage fp)
					{
						fp.Detail = new NavigationPage(new MainPage());
					}
				})
			});

			Content = layout;
		}
	}
}