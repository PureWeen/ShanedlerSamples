//#if ANDROID && NET8_0_OR_GREATER
//using Android.Content;
//using Android.Content.Res;
//using Android.Graphics.Drawables;
//using Google.Android.Material.Button;
//using Microsoft.Maui.Controls.Platform;
//using AView = Android.Views.View;
//using Microsoft.Maui.Graphics;
//using Android.Views;
//using Microsoft.Maui.Platform;
//using Microsoft.Maui.Handlers;
//using Microsoft.Maui.Controls.Handlers.Compatibility;

//namespace PureWeen.Maui.Controls.Handlers.Compatibility
//{
//#pragma warning disable RS0016 // Add public types and members to the declared API
//	public class ButtonRenderer : ViewRenderer<Button, MauiMaterialButton>, 
//		IButtonHandler, AView.IOnClickListener, AView.IOnTouchListener

//	{
//		public static IPropertyMapper<Button, IButtonHandler> Mapper
//			= new PropertyMapper<Button, IButtonHandler>(ButtonHandler.Mapper);

//		public static CommandMapper<Button, IButtonHandler> CommandMapper
//			= new CommandMapper<Button, IButtonHandler>(ButtonHandler.CommandMapper);

//		bool _isDisposed;

//		public ButtonRenderer(Context context) : base(context/*, Mapper, CommandMapper*/)
//		{
//			AutoPackage = false;
//		}

//		IButton IButtonHandler.VirtualView => Element!;

//		MaterialButton IButtonHandler.PlatformView => Control!;

//		ImageSourcePartLoader? _imageSourcePartLoader;
//		public ImageSourcePartLoader ImageSourceLoader =>
//			_imageSourcePartLoader ??= new ImageSourcePartLoader(this);

//		ImageSourcePartLoader IButtonHandler.ImageSourceLoader => ImageSourceLoader;


//		static ColorStateList TransparentColorStateList = Colors.Transparent.ToDefaultColorStateList();
//		protected override MauiMaterialButton CreateNativeControl()
//		{
//			return new MauiMaterialButton(Context!)
//			{
//				IconGravity = MaterialButton.IconGravityTextStart,
//				IconTintMode = Android.Graphics.PorterDuff.Mode.Add,
//				IconTint = TransparentColorStateList,
//				SoundEffectsEnabled = false
//			};
//		}

//		protected override void Dispose(bool disposing)
//		{
//			if (_isDisposed)
//				return;

//			_isDisposed = true;

//			if (disposing)
//			{
//				if (Control != null)
//				{
//					Control.SetOnClickListener(null);
//					Control.SetOnTouchListener(null);
//				}
//			}

//			base.Dispose(disposing);
//		}

//		public void SetImageSource(Drawable? obj)
//		{
//			Control!.Icon = obj;
//		}

//		protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
//		{
//			base.OnElementChanged(e);

//			if (e.NewElement != null)
//			{
//				if (Control == null)
//				{
//					var button = CreateNativeControl();

//					button.SetOnClickListener(this);
//					button.SetOnTouchListener(this);

//					SetNativeControl(button);
//				}
//			}
//		}

//		void IOnClickListener.OnClick(AView? v) => (Element as IButton)?.Clicked();

//		bool IOnTouchListener.OnTouch(AView? v, MotionEvent? e)
//		{
//			var button = (Element as IButton);
//			switch (e?.ActionMasked)
//			{
//				case MotionEventActions.Down:
//					button?.Pressed();
//					break;
//				case MotionEventActions.Cancel:
//				case MotionEventActions.Up:
//					button?.Released();
//					break;
//			}

//			return false;
//		}
//	}
//#pragma warning restore RS0016 // Add public types and members to the declared API
//}
//#endif