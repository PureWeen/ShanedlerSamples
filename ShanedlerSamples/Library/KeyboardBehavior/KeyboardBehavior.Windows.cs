#if WINDOWS
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.FixesAndWorkarounds
{
	public partial class KeyboardBehavior : PlatformBehavior<VisualElement>
	{
		protected override void OnAttachedTo(VisualElement bindable, FrameworkElement platformView)
		{
			base.OnAttachedTo(bindable, platformView);
			platformView.KeyDown += OnKeyDown;
			platformView.KeyUp += OnKeyUp;
			platformView.PreviewKeyDown += OnPreviewKeyDown;
			platformView.PreviewKeyUp += OnPreviewKeyUp;
		}

		protected override void OnDetachedFrom(VisualElement bindable, FrameworkElement platformView)
		{
			base.OnDetachedFrom(bindable, platformView);

			platformView.KeyDown -= OnKeyDown;
			platformView.KeyUp -= OnKeyUp;
			platformView.PreviewKeyDown -= OnPreviewKeyDown;
			platformView.PreviewKeyUp -= OnPreviewKeyUp;
		}

		void OnKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
		{

		}
		void OnPreviewKeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
		{

		}

		void OnPreviewKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
		{

		}

		void OnKeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
		{

		}
	}
}
#endif