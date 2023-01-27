#if ANDROID
using System;
using System.IO;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using Microsoft.Maui.Graphics;
using static Microsoft.Maui.Primitives.Dimension;
using AActivity = Android.App.Activity;
using AApplicationInfoFlags = Android.Content.PM.ApplicationInfoFlags;
using AAttribute = Android.Resource.Attribute;
using AColor = Android.Graphics.Color;
using Size = Microsoft.Maui.Graphics.Size;
using AView = Android.Views.View;

namespace Microsoft.Maui.Platform
{
    public static class ContextExtensionsWorkAround
    {
        // Caching this display density here means that all pixel calculations are going to be based on the density
        // of the first Context these extensions are run against. That's probably fine, but if we run into a 
        // situation where subsequent activities can be launched with a different display density from the initial
        // activity, we'll need to remove this cached value or cache it in a Dictionary<Context, float>
        static float s_displayDensity = float.MinValue;

        static int? _actionBarHeight;
        static int? _statusBarHeight;
        static int? _navigationBarHeight;
        // TODO FromPixels/ToPixels is both not terribly descriptive and also possibly sort of inaccurate?
        // These need better names. It's really To/From Device-Independent, but that doesn't exactly roll off the tongue.

        internal static double FromPixels(this AView view, double pixels)
        {
            if (s_displayDensity != float.MinValue)
                return pixels / s_displayDensity;
            return view.Context.FromPixels(pixels);
        }

        internal static Size FromPixels(this AView view, double width, double height)
        {
            return new Size(view.FromPixels(width), view.FromPixels(height));
        }

        internal static float ToPixels(this AView view, double dp)
        {
            if (s_displayDensity != float.MinValue)
                return (float)Math.Ceiling(dp * s_displayDensity);
            return view.Context.ToPixels(dp);
        }

        internal static int GetDisabledThemeAttrColor(this Context context, int attr)
        {
            if (context.Theme == null)
                return 0;

            using (var value = new TypedValue())
            {
                // Now retrieve the disabledAlpha value from the theme
                context.Theme.ResolveAttribute(AAttribute.DisabledAlpha, value, true);
                float disabledAlpha = value.Float;
                return GetThemeAttrColor(context, attr, disabledAlpha);
            }
        }

        internal static int GetThemeAttrColor(this Context context, int attr)
        {
            using (TypedValue mTypedValue = new TypedValue())
            {
                if (context.Theme?.ResolveAttribute(attr, mTypedValue, true) == true)
                {
                    if (mTypedValue.Type >= DataType.FirstInt && mTypedValue.Type <= DataType.LastInt)
                    {
                        return mTypedValue.Data;
                    }
                    else if (mTypedValue.Type == DataType.String)
                    {
                        if (context.Resources != null)
                        {
                            if (OperatingSystem.IsAndroidVersionAtLeast(23))
                                return context.Resources.GetColor(mTypedValue.ResourceId, context.Theme);
                            else
#pragma warning disable CS0618 // Type or member is obsolete
                                return context.Resources.GetColor(mTypedValue.ResourceId);
#pragma warning restore CS0618 // Type or member is obsolete
                        }
                    }
                }
            }

            return 0;
        }

        internal static int GetThemeAttrColor(this Context context, int attr, float alpha)
        {
            int color = GetThemeAttrColor(context, attr);
            int originalAlpha = AColor.GetAlphaComponent(color);
            // Return the color, multiplying the original alpha by the disabled value
            return (color & 0x00ffffff) | ((int)Math.Round(originalAlpha * alpha) << 24);
        }

        static void EnsureMetrics(Context? context)
        {
            if (s_displayDensity != float.MinValue)
                return;

            context ??= Android.App.Application.Context;

            using (DisplayMetrics? metrics = context.Resources?.DisplayMetrics)
                s_displayDensity = metrics != null ? metrics.Density : 1;
        }

        internal static Context? GetThemedContext(this Context context)
        {
            if (context == null)
                return null;

            if (context is AppCompatActivity activity)
                return activity.SupportActionBar?.ThemedContext ?? context;

            if (context is ContextWrapper contextWrapper)
                return contextWrapper.BaseContext?.GetThemedContext();

            return null;
        }

        internal static Color GetAccentColor(this Context context)
        {
            Color? rc = null;
            using (var value = new TypedValue())
            {
                if (context.Theme != null)
                {
                    if (context.Theme.ResolveAttribute(global::Android.Resource.Attribute.ColorAccent, value, true)) // Android 5.0+
                    {
                        rc = Color.FromUint((uint)value.Data);
                    }
                }
            }

            return rc ?? Color.FromArgb("#ff33b5e5");
        }

        internal static int GetStatusBarHeight(this Context context)
        {
            if (_statusBarHeight != null)
                return _statusBarHeight.Value;

            var resources = context.Resources;

            if (resources == null)
                return 0;

            int resourceId = resources.GetIdentifier("status_bar_height", "dimen", "android");
            if (resourceId > 0)
            {
                _statusBarHeight = resources.GetDimensionPixelSize(resourceId);
            }

            return _statusBarHeight ?? 0;
        }

        internal static int GetNavigationBarHeight(this Context context)
        {
            if (_navigationBarHeight != null)
                return _navigationBarHeight.Value;

            var resources = context.Resources;

            if (resources == null)
                return 0;

            int resourceId = resources.GetIdentifier("navigation_bar_height", "dimen", "android");
            if (resourceId > 0)
            {
                _navigationBarHeight = resources.GetDimensionPixelSize(resourceId);
            }

            return _navigationBarHeight ?? 0;
        }

        internal static int CreateMeasureSpec(this Context context, double constraint, double explicitSize, double maximumSize)
        {
            var mode = MeasureSpecMode.AtMost;

            if (IsExplicitSet(explicitSize))
            {
                // We have a set value (i.e., a Width or Height)
                mode = MeasureSpecMode.Exactly;
                constraint = explicitSize;
            }
            else if (IsMaximumSet(maximumSize))
            {
                mode = MeasureSpecMode.AtMost;
                constraint = maximumSize;
            }
            else if (double.IsInfinity(constraint))
            {
                // We've got infinite space; we'll leave the size up to the platform control
                mode = MeasureSpecMode.Unspecified;
                constraint = 0;
            }

            // Convert to a platform size to create the spec for measuring
            var deviceConstraint = (int)context.ToPixels(constraint);

            return mode.MakeMeasureSpec(deviceConstraint);
        }

        internal static bool IsDestroyed(this Context? context)
        {
            if (context == null)
                return true;

            if (context.GetActivity() is FragmentActivity fa)
            {
                if (fa.IsDisposed())
                    return true;

                var stateCheck = AndroidX.Lifecycle.Lifecycle.State.Destroyed;

                if (stateCheck != null &&
                    fa.Lifecycle.CurrentState == stateCheck)
                {
                    return true;
                }

                if (fa.IsDestroyed)
                    return true;
            }

            return context.IsDisposed();
        }

        internal static bool IsPlatformContextDestroyed(this IElementHandler? handler)
        {
            var context = handler?.MauiContext?.Context;
            return context.IsDestroyed();
        }
    }
}
#endif