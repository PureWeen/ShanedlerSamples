using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Maui.FixesAndWorkarounds
{
	// https://github.com/dotnet/maui/issues/9300
	public class ShellContentDI : ShellContent
	{
		Shell Shell => Parent?.Parent?.Parent as Shell;
		ShellContent CurrentShellContent => Shell?.CurrentItem?.CurrentItem?.CurrentItem;

		protected override void OnAppearing()
		{
			base.OnAppearing();
			Shell.Navigated -= OnShellNavigated;
			Shell.Navigated += OnShellNavigated;
		}

		private void OnShellNavigated(object sender, ShellNavigatedEventArgs e)
		{
			if (this != CurrentShellContent)
			{
				var property = typeof(ShellContent)
						.GetProperty("ContentCache", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

				property.SetValue(this, null);
				Shell.Navigated -= OnShellNavigated;
			}
		}
	}
}
