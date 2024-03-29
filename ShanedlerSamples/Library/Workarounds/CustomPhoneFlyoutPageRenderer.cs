﻿//#nullable disable
//#if IOS || MACCATALYST
//using System;
//using System.ComponentModel;
//using System.Linq;
//using System.Reflection;
//using Maui.FixesAndWorkarounds.Library.Common;
//using Microsoft.Maui.Controls.Internals;
//using Microsoft.Maui.Controls.Platform;
//using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
//using Microsoft.Maui.Graphics;
//using Microsoft.Maui.Handlers;
//using Microsoft.Maui.Platform;
//using ObjCRuntime;
//using UIKit;
//using PointF = CoreGraphics.CGPoint;

//namespace Microsoft.Maui.Controls.Handlers.Compatibility
//{
//	public class CustomPhoneFlyoutPageRenderer : UIViewController, IPlatformViewHandler
//	{

//		UIView _clickOffView;
//		UIViewController _detailController;
//		VisualElement _element;
//		bool _disposed;

//		UIViewController _flyoutController;

//		UIPanGestureRecognizer _panGesture;

//		bool _presented;
//		UIGestureRecognizer _tapGesture;

//		bool _applyShadow;

//		Page Page => Element as Page;
//		IFlyoutPageController FlyoutPageController => FlyoutPage;
//		IMauiContext _mauiContext;
//		IMauiContext MauiContext => _mauiContext;
//		bool IsPad => UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad;

//		public static IPropertyMapper<FlyoutPage, CustomPhoneFlyoutPageRenderer> Mapper = new PropertyMapper<FlyoutPage, CustomPhoneFlyoutPageRenderer>(ViewHandler.ViewMapper);
//		public static CommandMapper<FlyoutPage, CustomPhoneFlyoutPageRenderer> CommandMapper = new CommandMapper<FlyoutPage, CustomPhoneFlyoutPageRenderer>(ViewHandler.ViewCommandMapper);
//		ViewHandlerDelegator<FlyoutPage> _viewHandlerWrapper;

//		[Preserve(Conditional = true)]
//		public CustomPhoneFlyoutPageRenderer()
//		{
//			_viewHandlerWrapper = new ViewHandlerDelegator<FlyoutPage>(Mapper, CommandMapper, this);
//		}

//		FlyoutPage FlyoutPage => Element as FlyoutPage;

//		bool Presented
//		{
//			get { return _presented; }
//		}

//		public VisualElement Element => _viewHandlerWrapper.Element ?? _element;

//		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

//		public Size GetDesiredSize(double widthConstraint, double heightConstraint)
//		{
//			return this.GetDesiredSizeFromHandler(widthConstraint, heightConstraint);
//		}

//		public UIView NativeView
//		{
//			get { return View; }
//		}

//		public void SetElement(VisualElement element)
//		{

//			var flyoutPage = element as FlyoutPage;

//			_flyoutController = new ChildViewController();
//			_detailController = new ChildViewController();

//			_clickOffView = new UIView();
//			_clickOffView.BackgroundColor = new Color(0, 0, 0, 0).ToPlatform();
//			_viewHandlerWrapper.SetVirtualView(element, OnElementChanged, false);
//			_element = element;
//			UpdatePresented(((FlyoutPage)element).IsPresented);
//			Element.SizeChanged += PageOnSizeChanged;
//		}

//		public void SetElementSize(Size size)
//		{
//			Element.Layout(new Rect(Element.X, Element.Y, size.Width, size.Height));
//		}

//		public UIViewController ViewController
//		{
//			get { return this; }
//		}

//		public override void ViewDidAppear(bool animated)
//		{
//			base.ViewDidAppear(animated);
//			Page.SendAppearing();
//		}

//		public override void ViewDidDisappear(bool animated)
//		{
//			base.ViewDidDisappear(animated);
//			Page?.SendDisappearing();
//		}

//		public override void ViewDidLayoutSubviews()
//		{
//			base.ViewDidLayoutSubviews();

//			if (Element is IView view &&
//				!Primitives.Dimension.IsExplicitSet(view.Width) &&
//				!Primitives.Dimension.IsExplicitSet(view.Height))
//			{
//				view.Arrange(new Rect(Element.X, Element.Y, View.Bounds.Width, View.Bounds.Height));
//			}

//			LayoutChildren(false);
//		}

//		public override void ViewDidLoad()
//		{
//			base.ViewDidLoad();

//			((FlyoutPage)Element).PropertyChanged += HandlePropertyChanged;

//			bool shouldReceive(UIGestureRecognizer g, UITouch t)
//			{
//				return !FlyoutPageController.ShouldShowSplitMode && Presented;
//			}

//			_tapGesture = new UITapGestureRecognizer(() =>
//			{
//				UpdatePresented(false, true);
//			});

//			if (IsPad)
//				_tapGesture.ShouldReceiveTouch = shouldReceive;

//			_clickOffView.AddGestureRecognizer(_tapGesture);

//			PackContainers();
//			UpdateFlyoutPageContainers();

//			UpdateBackground();

//			UpdatePanGesture();
//			UpdateApplyShadow(((FlyoutPage)Element).OnThisPlatform().GetApplyShadow());
//		}

//		[System.Runtime.Versioning.UnsupportedOSPlatform("ios8.0")]
//		[System.Runtime.Versioning.UnsupportedOSPlatform("tvos")]
//		public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
//		{
//			base.WillRotate(toInterfaceOrientation, duration);

//			if (IsPad)
//			{
//				if (FlyoutPageController.ShouldShowSplitMode)
//					UpdatePresented(true);
//				else
//					UpdatePresented(false);
//			}
//			else
//			{
//				if (!FlyoutPageController.ShouldShowSplitMode && _presented)
//					UpdatePresented(false);
//			}
//		}

//		void UpdatePresented(bool newValue, bool animated = false)
//		{
//			if (Presented == newValue)
//				return;

//			if (!newValue && FlyoutPageController.ShouldShowSplitMode)
//			{
//				return;
//			}

//			_presented = newValue;

//			LayoutChildren(animated);
//			UpdateClickOffView();

//			ToggleAccessibilityElementsHidden();

//			((IElementController)Element).SetValueFromRenderer(FlyoutPage.IsPresentedProperty, _presented);
//		}

//		protected override void Dispose(bool disposing)
//		{
//			if (disposing && !_disposed)
//			{
//				Element.SizeChanged -= PageOnSizeChanged;
//				Element.PropertyChanged -= HandlePropertyChanged;

//				if (_tapGesture != null)
//				{
//					if (_clickOffView != null && _clickOffView.GestureRecognizers.Contains(_tapGesture))
//					{
//						_clickOffView.GestureRecognizers.Remove(_tapGesture);
//						_clickOffView.Dispose();
//					}
//					_tapGesture.Dispose();
//				}
//				if (_panGesture != null)
//				{
//					if (View != null && View.GestureRecognizers.Contains(_panGesture))
//						View.GestureRecognizers.Remove(_panGesture);
//					_panGesture.Dispose();
//				}

//				EmptyContainers();

//				Page.SendDisappearing();

//				_disposed = true;
//			}

//			base.Dispose(disposing);
//		}

//		protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
//		{
//			var changed = ElementChanged;
//			if (changed != null)
//				changed(this, e);
//		}

//		void UpdateClickOffView()
//		{
//			if (IsPad && FlyoutPageController.ShouldShowSplitMode)
//			{
//				RemoveClickOffView();
//				return;
//			}

//			if (Presented)
//				AddClickOffView();
//			else
//				RemoveClickOffView();
//		}

//		void AddClickOffView()
//		{
//			if (_clickOffView.Superview == View)
//				return;

//			View.Add(_clickOffView);
//			_clickOffView.Frame = _detailController.View.Frame;
//		}

//		void RemoveClickOffView()
//		{
//			_clickOffView.RemoveFromSuperview();
//		}

//		void EmptyContainers()
//		{
//			foreach (var child in _detailController.View.Subviews.Concat(_flyoutController.View.Subviews))
//				child.RemoveFromSuperview();

//			foreach (var vc in _detailController.ChildViewControllers.Concat(_flyoutController.ChildViewControllers))
//				vc.RemoveFromParentViewController();
//		}

//		void HandleFlyoutPropertyChanged(object sender, PropertyChangedEventArgs e)
//		{
//			if (e.PropertyName == Page.IconImageSourceProperty.PropertyName || e.PropertyName == Page.TitleProperty.PropertyName)
//				UpdateLeftBarButton();
//		}

//		void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
//		{
//			if (e.PropertyName == "Flyout" || e.PropertyName == "Detail")
//				UpdateFlyoutPageContainers();
//			else if (e.PropertyName == Microsoft.Maui.Controls.FlyoutPage.IsPresentedProperty.PropertyName)
//				UpdatePresented(((FlyoutPage)Element).IsPresented, true);
//			else if (e.PropertyName == Microsoft.Maui.Controls.FlyoutPage.IsGestureEnabledProperty.PropertyName)
//				UpdatePanGesture();
//			else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName || e.PropertyName == VisualElement.BackgroundProperty.PropertyName)
//				UpdateBackground();
//			else if (e.PropertyName == Page.BackgroundImageSourceProperty.PropertyName)
//				UpdateBackground();
//			else if (e.PropertyName == PlatformConfiguration.iOSSpecific.FlyoutPage.ApplyShadowProperty.PropertyName)
//				UpdateApplyShadow(((FlyoutPage)Element).OnThisPlatform().GetApplyShadow());
//		}

//		void LayoutChildren(bool animated)
//		{
//			var frame = Element.Bounds.ToCGRect();
//			var flyoutFrame = frame;
//			nfloat opacity = 1;

//			if (IsPad)
//				flyoutFrame.Width = 320;
//			else
//				flyoutFrame.Width = (int)(Math.Min(flyoutFrame.Width, flyoutFrame.Height) * 0.8);

//			var detailRenderer = FlyoutPage.Detail.Handler as IPlatformViewHandler;
//			if (detailRenderer == null)
//				return;
//			var detailView = detailRenderer.ViewController.View;

//			var isRTL = (Element as IVisualElementController)?.EffectiveFlowDirection.IsRightToLeft() == true;
//			if (isRTL)
//			{
//				flyoutFrame.X = (int)(flyoutFrame.Width * .25);
//			}

//			_flyoutController.View.Frame = flyoutFrame;

//			var target = frame;
//			if (Presented)
//			{
//				target.X += flyoutFrame.Width;

//				if (IsPad && FlyoutPageController.ShouldShowSplitMode)
//					target.Width -= flyoutFrame.Width;

//				if (_applyShadow)
//					opacity = 0.5f;
//			}

//			if (isRTL)
//			{
//				target.X = target.X * -1;
//			}

//			if (animated)
//			{
//				UIView.Animate(0.250, 0, UIViewAnimationOptions.CurveEaseOut, () =>
//				{
//					var view = _detailController.View;
//					view.Frame = target;
//					detailView.Layer.Opacity = (float)opacity;
//				}, () => { });
//			}
//			else
//			{
//				_detailController.View.Frame = target;
//				detailView.Layer.Opacity = (float)opacity;
//			}

//			FlyoutPageController.FlyoutBounds = new Rect(flyoutFrame.X, 0, flyoutFrame.Width, flyoutFrame.Height);

//			if (IsPad)
//				FlyoutPageController.DetailBounds = new Rect(target.X, 0, frame.Width, frame.Height);
//			else
//				FlyoutPageController.DetailBounds = new Rect(0, 0, frame.Width, frame.Height);

//			if (Presented)
//				_clickOffView.Frame = _detailController.View.Frame;
//		}

//		void PackContainers()
//		{
//			_detailController.View.BackgroundColor = new UIColor(1, 1, 1, 1);
//			View.AddSubview(_flyoutController.View);
//			View.AddSubview(_detailController.View);

//			AddChildViewController(_flyoutController);
//			AddChildViewController(_detailController);
//		}

//		void PageOnSizeChanged(object sender, EventArgs eventArgs)
//		{
//			LayoutChildren(false);
//		}

//		void UpdateBackground()
//		{
//			((Page)(Element)).BackgroundImageSource.LoadImage(MauiContext, result =>
//			{
//				var bgImage = result?.Value;
//				if (bgImage != null)
//					View.BackgroundColor = UIColor.FromPatternImage(bgImage);
//				else
//				{
//					Brush background = Element.Background;

//					if (!Brush.IsNullOrEmpty(background))
//						View.UpdateBackground(Element.Background);
//					else
//					{
//						if (Element.BackgroundColor == null)
//							View.BackgroundColor = UIColor.White;
//						else
//							View.BackgroundColor = Element.BackgroundColor.ToPlatform();
//					}
//				}
//			});
//		}

//		void UpdateFlyoutPageContainers()
//		{
//			((FlyoutPage)Element).Flyout.PropertyChanged -= HandleFlyoutPropertyChanged;

//			EmptyContainers();

//			var flyoutRenderer = ((FlyoutPage)Element).Flyout.ToHandler(MauiContext);
//			var detailRenderer = ((FlyoutPage)Element).Detail.ToHandler(MauiContext);

//			((FlyoutPage)Element).Flyout.PropertyChanged += HandleFlyoutPropertyChanged;

//			UIView flyoutView = flyoutRenderer.ViewController.View;

//			_flyoutController.View.AddSubview(flyoutView);
//			_flyoutController.AddChildViewController(flyoutRenderer.ViewController);

//			UIView detailView = detailRenderer.ViewController.View;

//			_detailController.View.AddSubview(detailView);
//			_detailController.AddChildViewController(detailRenderer.ViewController);

//			SetNeedsStatusBarAppearanceUpdate();
//			if (OperatingSystem.IsIOSVersionAtLeast(11))
//				SetNeedsUpdateOfHomeIndicatorAutoHidden();

//			if (detailRenderer.ViewController.View.Superview != null)
//				detailRenderer.ViewController.View.Superview.BackgroundColor = Microsoft.Maui.Graphics.Colors.Black.ToPlatform();

//			ToggleAccessibilityElementsHidden();
//		}

//		void UpdateLeftBarButton()
//		{
//			var FlyoutPage = Element as FlyoutPage;
//			if (!(FlyoutPage?.Detail is NavigationPage))
//				return;

//			var detailRenderer =
//				(FlyoutPage.Detail?.Handler as IPlatformViewHandler)
//				?.ViewController as UINavigationController;

//			UIViewController firstPage = detailRenderer?.ViewControllers.FirstOrDefault();
//			if (firstPage != null)
//			{
//				var method = typeof(NavigationRenderer)
//					.GetMethod("SetFlyoutLeftBarButton",
//					BindingFlags.NonPublic | BindingFlags.Static);

//				method?.Invoke(null, new object[] { firstPage, FlyoutPage });
//			}
//		}

//		void UpdateApplyShadow(bool value)
//		{
//			_applyShadow = value;
//		}

//		public override UIViewController ChildViewControllerForStatusBarHidden()
//		{
//			if (((FlyoutPage)Element).Detail?.Handler is IPlatformViewHandler nvh)
//				return nvh.ViewController;
//			else
//				return base.ChildViewControllerForStatusBarHidden();
//		}

//		public override UIViewController ChildViewControllerForHomeIndicatorAutoHidden
//		{
//			get
//			{
//				if (((FlyoutPage)Element).Detail?.Handler is IPlatformViewHandler nvh)
//					return nvh.ViewController;
//				else
//					return base.ChildViewControllerForStatusBarHidden();
//			}
//		}

//		void ToggleAccessibilityElementsHidden()
//		{
//			var flyoutView = _flyoutController?.View;
//			if (flyoutView != null)
//				flyoutView.AccessibilityElementsHidden = !Presented;

//			var detailView = _detailController?.View;
//			if (detailView != null)
//				detailView.AccessibilityElementsHidden = Presented;
//		}

//		void UpdatePanGesture()
//		{
//			var model = (FlyoutPage)Element;
//			if (!model.IsGestureEnabled)
//			{
//				if (_panGesture != null)
//					View.RemoveGestureRecognizer(_panGesture);
//				return;
//			}

//			if (_panGesture != null)
//			{
//				View.AddGestureRecognizer(_panGesture);
//				return;
//			}

//			bool shouldReceive(UIGestureRecognizer g, UITouch t)
//			{
//				return !(t.View is UISlider) &&
//					!IsSwipeView(t.View) &&
//					!FlyoutPageController.ShouldShowSplitMode;
//			}

//			var center = new PointF();
//			_panGesture = new UIPanGestureRecognizer(g =>
//			{
//				var isRTL = (Element as IVisualElementController)?.EffectiveFlowDirection.IsRightToLeft() == true;

//				int directionModifier = isRTL ? -1 : 1;

//				switch (g.State)
//				{
//					case UIGestureRecognizerState.Began:
//						center = g.LocationInView(g.View);
//						break;
//					case UIGestureRecognizerState.Changed:
//						var currentPosition = g.LocationInView(g.View);
//						var motion = currentPosition.X - center.X;

//						motion = motion * directionModifier;

//						var detailView = _detailController.View;
//						var targetFrame = detailView.Frame;
//						if (Presented)
//							targetFrame.X = (nfloat)Math.Max(0, _flyoutController.View.Frame.Width + Math.Min(0, motion));
//						else
//							targetFrame.X = (nfloat)Math.Min(_flyoutController.View.Frame.Width, Math.Max(0, motion));

//						targetFrame.X = targetFrame.X * directionModifier;
//						if (_applyShadow)
//						{
//							var openProgress = targetFrame.X / _flyoutController.View.Frame.Width;
//							ApplyDetailShadow((nfloat)openProgress);
//						}

//						detailView.Frame = targetFrame;
//						break;
//					case UIGestureRecognizerState.Ended:
//						var detailFrame = _detailController.View.Frame;
//						var flyoutFrame = _flyoutController.View.Frame;
//						if (Presented)
//						{
//							if (detailFrame.X * directionModifier < flyoutFrame.Width * .75)
//								UpdatePresented(false);
//							else
//								LayoutChildren(true);
//						}
//						else
//						{
//							if (detailFrame.X * directionModifier > flyoutFrame.Width * .25)
//								UpdatePresented(true);
//							else
//								LayoutChildren(true);
//						}
//						break;
//				}
//			});
//			_panGesture.CancelsTouchesInView = false;
//			_panGesture.ShouldReceiveTouch = shouldReceive;
//			_panGesture.MaximumNumberOfTouches = 2;
//			View.AddGestureRecognizer(_panGesture);
//		}

//		static bool IsSwipeView(UIView view)
//		{
//			if (view == null)
//				return false;

//			if (view.Superview is MauiSwipeView)
//				return true;

//			return IsSwipeView(view.Superview);
//		}

//		class ChildViewController : UIViewController
//		{
//			public override void ViewDidLayoutSubviews()
//			{
//				foreach (var vc in ChildViewControllers)
//					vc.View.Frame = View.Bounds;
//			}
//		}

//		void ApplyDetailShadow(nfloat percent)
//		{
//			var detailView = ((IPlatformViewHandler)FlyoutPage.Detail.Handler).ViewController.View;
//			var opacity = (nfloat)(0.5 + (0.5 * (1 - percent)));
//			detailView.Layer.Opacity = (float)opacity;
//		}


//		#region IPlatformViewHandler
//		bool IViewHandler.HasContainer { get => false; set { } }

//		object IViewHandler.ContainerView => null;

//		IView IViewHandler.VirtualView => Element;

//		object IElementHandler.PlatformView => NativeView;

//		Maui.IElement IElementHandler.VirtualView => Element;

//		IMauiContext IElementHandler.MauiContext => _mauiContext;

//		UIView IPlatformViewHandler.PlatformView => NativeView;

//		UIView IPlatformViewHandler.ContainerView => null;

//		UIViewController IPlatformViewHandler.ViewController => this;

//		void IViewHandler.PlatformArrange(Rect rect) =>
//			_viewHandlerWrapper.PlatformArrange(rect);

//		void IElementHandler.SetMauiContext(IMauiContext mauiContext)
//		{
//			_mauiContext = mauiContext;
//		}

//		void IElementHandler.SetVirtualView(Maui.IElement view)
//		{
//			SetElement((VisualElement)view);
//		}

//		void IElementHandler.UpdateValue(string property)
//		{
//			_viewHandlerWrapper.UpdateProperty(property);
//		}

//		void IElementHandler.Invoke(string command, object args)
//		{
//			_viewHandlerWrapper.Invoke(command, args);
//		}

//		void IElementHandler.DisconnectHandler()
//		{
//			_viewHandlerWrapper.DisconnectHandler();
//		}
//		#endregion
//	}
//}
//#endif