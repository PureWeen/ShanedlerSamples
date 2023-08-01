namespace ShanedlerSamples;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState activationState)
	{
		return new KeyboardCapableWindow(new AppShell());
	}


	class KeyboardCapableWindow : Window
	{
		public KeyboardCapableWindow()
		{
		}

		public KeyboardCapableWindow(Page page) : base(page)
		{
			page.HandlerChanged += OnPageHandlerChanged;
		}

		void OnPageHandlerChanged(object sender, EventArgs e)
		{

			if (sender is IView view && view.Handler is IPlatformViewHandler pvh)
			{
#if WINDOWS
				pvh.PlatformView.KeyDown += (X, Y) =>
				{

				};
				pvh.PlatformView.KeyUp += (X, Y) =>
				{

				};
				pvh.PlatformView.PreviewKeyDown += (X, Y) =>
				{

				};

				pvh.PlatformView.PreviewKeyUp += (X, Y) =>
				{

				};
#endif
			}
		}

		protected override void OnHandlerChanged()
		{
			base.OnHandlerChanged();
		}
	}
}
