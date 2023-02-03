#if ANDROID
#nullable disable
using System;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.Fragment.App;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific.AppCompat;
using Microsoft.Maui.Platform;
using AView = Android.Views.View;

namespace Microsoft.Maui.Controls.Platform
{
	internal class FragmentContainer : Fragment
	{
		readonly WeakReference _pageRenderer;
		readonly IMauiContext _mauiContext;
		Action<AView> _onCreateCallback;
		AView _pageContainer;
		ViewGroup _parent;

		public FragmentContainer(Page page, IMauiContext mauiContext)
		{
			_pageRenderer = new WeakReference(page);
			_mauiContext = mauiContext;
		}

		public virtual Page Page => (Page)_pageRenderer?.Target;

		public static FragmentContainer CreateInstance(Page page, IMauiContext mauiContext)
		{
			return new FragmentContainer(page, mauiContext) { Arguments = new Bundle() };
		}

		public void SetOnCreateCallback(Action<AView> callback)
		{
			_onCreateCallback = callback;
		}

		public override AView OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			_parent = container ?? _parent;

			if (Page != null)
			{
				_pageContainer = Page.ToPlatform(_mauiContext, RequireContext(), inflater, ChildFragmentManager);
				_parent = _parent ?? (_pageContainer.Parent as ViewGroup);
				_onCreateCallback?.Invoke(_pageContainer);

				return _pageContainer;
			}

			return null;
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			Page?.Handler?.DisconnectHandler();
		}

		public override void OnResume()
		{
			if (_pageContainer == null)
				return;

			_parent = (_pageContainer.Parent as ViewGroup) ?? _parent;
			if (_pageContainer.Parent == null && _parent != null)
			{
				// Re-add the view to the container if Android removed it
				// Because we are re-using views inside OnCreateView Android
				// will remove the "previous" view from the parent but since our
				// "previous" view and "current" view are the same we have to re-add it
				_parent.AddView(_pageContainer);
			}

			base.OnResume();
		}
	}
}
#endif
