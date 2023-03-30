#if ANDROID
using Maui.FixesAndWorkarounds.Library.Common;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Handlers.Compatibility;

using Microsoft.Maui.Handlers;
using Microsoft.Maui.LifecycleEvents;
using System.Reflection;

namespace Maui.FixesAndWorkarounds
{
	public static partial class HostExtensions
	{
		public static MauiAppBuilder ConfigureFlyoutPageWorkarounds(this MauiAppBuilder builder)
		{

			FlyoutViewHandler.Mapper.AppendToMapping(nameof(IFlyoutView.Detail), OnFixDetailsPage);
			FlyoutViewHandler.Mapper.AppendToMapping(nameof(IFlyoutView.Flyout), OnFixDetailsPage);
			return builder;
		}

		static void OnFixDetailsPage(IFlyoutViewHandler arg1, IFlyoutView arg2)
		{
			var VirtualView = arg1.VirtualView;

			if (VirtualView.FlyoutBehavior != FlyoutBehavior.Locked)
				return;

			arg1.InvokePrivateMethod("UpdateDetailsFragmentView", null);
		}
	}

}

#endif
