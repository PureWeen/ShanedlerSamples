﻿#if ANDROID
using System;
using System.Reflection;
using Android.Content;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using Java.Util.Zip;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Devices;
using AView = Android.Views.View;

namespace Microsoft.Maui.Platform
{
    internal static partial class MauiContextExtensions
    {
        public static NavigationRootManager GetNavigationRootManager(this IMauiContext mauiContext) =>
            mauiContext.Services.GetRequiredService<NavigationRootManager>();

        public static LayoutInflater GetLayoutInflater(this IMauiContext mauiContext)
        {
            var layoutInflater = mauiContext.Services.GetService<LayoutInflater>();

            if (!layoutInflater.IsAlive() && mauiContext.Context != null)
            {
                var activity = mauiContext.Context.GetActivity();

                if (activity != null)
                    layoutInflater = LayoutInflater.From(activity);
            }

            return layoutInflater ?? throw new InvalidOperationException("LayoutInflater Not Found");
        }

        public static FragmentManager GetFragmentManager(this IMauiContext mauiContext)
        {
            var fragmentManager = mauiContext.Services.GetService<FragmentManager>();

            return fragmentManager
                ?? mauiContext.Context?.GetFragmentManager()
                ?? throw new InvalidOperationException("FragmentManager Not Found");
        }

        public static AppCompatActivity GetActivity(this IMauiContext mauiContext) =>
            (mauiContext.Context?.GetActivity() as AppCompatActivity)
            ?? throw new InvalidOperationException("AppCompatActivity Not Found");


        internal static IServiceProvider GetApplicationServices(this IMauiContext mauiContext)
        {
            if (mauiContext.Context?.ApplicationContext is MauiApplication ma)
                return ma.Services;

            throw new InvalidOperationException("Unable to find Application Services");
        }

        public static Android.App.Activity GetPlatformWindow(this IMauiContext mauiContext) =>
            mauiContext.Services.GetRequiredService<Android.App.Activity>();

        internal static AView ToPlatform(
            this IView view,
            IMauiContext fragmentMauiContext,
            Android.Content.Context context,
            LayoutInflater layoutInflater,
            FragmentManager childFragmentManager)
        {
            if (view.Handler?.MauiContext is MauiContext scopedMauiContext)
            {
                // If this handler becomes to a different activity then we need to 
                // recreate the view.
                // If it's the same activity we just update the layout inflater
                // and the fragment manager so that the platform view doesn't recreate
                // underneath the users feet
                if (scopedMauiContext.GetActivity() == context.GetActivity() &&
                    view.Handler.PlatformView is AView platformView)
                {
                    var method = scopedMauiContext.GetType().GetMethod("AddWeakSpecific", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                    scopedMauiContext.AddWeakSpecific(layoutInflater);
                    scopedMauiContext.AddWeakSpecific(childFragmentManager);
                    return platformView;
                }
            }

            return view.ToPlatform(fragmentMauiContext.MakeScoped(layoutInflater: layoutInflater, fragmentManager: childFragmentManager));
        }

        public static IMauiContext MakeScoped(this IMauiContext mauiContext,
            LayoutInflater? layoutInflater = null,
            FragmentManager? fragmentManager = null,
            Android.Content.Context? context = null,
            bool registerNewNavigationRoot = false)
        {
            var scopedContext = new MauiContext(mauiContext.Services);

            if (layoutInflater != null)
                scopedContext.AddWeakSpecific(layoutInflater);

            if (fragmentManager != null)
                scopedContext.AddWeakSpecific(fragmentManager);

            if (context != null)
                scopedContext.AddWeakSpecific(context);

            if (registerNewNavigationRoot)
            {
                if (fragmentManager == null)
                    throw new InvalidOperationException("If you're creating a new Navigation Root you need to use a new Fragment Manager");

                scopedContext.AddSpecific(new NavigationRootManager(scopedContext));
            }

            return scopedContext;
        }


        public static IMauiContext AddWeakSpecific<T>(this IMauiContext mauiContext, T thing)
            where T : class
        {
            var t = mauiContext.GetType();
            var methods = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var method = methods.Where(m => m.IsGenericMethod && m.Name.Equals("AddWeakSpecific", StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

            if (method != null)
            {
                MethodInfo generic = method.MakeGenericMethod(typeof(T));
                generic.Invoke(obj: mauiContext, parameters: new object[] { thing });
            }
            return mauiContext;
        }

        public static IMauiContext AddSpecific<T>(this IMauiContext mauiContext, T thing)
            where T : class
        {
            var t = mauiContext.GetType();
            var methods = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var method = methods.Where(m => m.IsGenericMethod && m.Name.Equals("AddSpecific", StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

            if (method != null)
            {
                MethodInfo generic = method.MakeGenericMethod(typeof(T));
                generic.Invoke(obj: mauiContext, parameters: new object[] { thing });
            }

            method.Invoke(mauiContext, new[] { thing });
            return mauiContext;
        }
    }
}
#endif