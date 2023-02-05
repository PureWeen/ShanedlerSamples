#if ANDROID
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AndroidX.Fragment.App;
using AndroidX.ViewPager2.Adapter;
using Microsoft.Maui.Platform;
using AView = Android.Views.View;

namespace Microsoft.Maui.Controls.Platform
{
    internal class MultiPageFragmentStateAdapter<T> : FragmentStateAdapter where T : Page
    {
        MultiPage<T> _page;
        readonly IMauiContext _context;
        List<AdapterItemKey> keys = new List<AdapterItemKey>();

        public MultiPageFragmentStateAdapter(
            MultiPage<T> page, FragmentManager fragmentManager, IMauiContext context)
            : base(fragmentManager, context.GetActivity().Lifecycle)
        {
            _page = page;
            _context = context;
        }

        public override int ItemCount => CountOverride;

        public int CountOverride { get; set; }

        public override Fragment CreateFragment(int position)
        {
            var fragment = FragmentContainer.CreateInstance(GetItemIdByPosition(position), _context);
            return fragment;
        }

        public override long GetItemId(int position)
        {
            return GetItemIdByPosition(position).ItemId;
        }

        public override bool ContainsItem(long itemId)
        {
            return GetItemByItemId(itemId) != null;
        }

        AdapterItemKey GetItemIdByPosition(int position)
        {
            CheckItemKeys();
            var page = _page.Children[position];
            for (var i = 0; i < keys.Count; i++)
            {
                var item = keys[i];
                if (item.Page == page)
                {
                    return item;
                }
            }

            var itemKey = new AdapterItemKey(page, (ik) => keys.Remove(ik));
            keys.Add(itemKey);

            return itemKey;
        }

        AdapterItemKey? GetItemByItemId(long itemId)
        {
            CheckItemKeys();
            for (var i = 0; i < keys.Count; i++)
            {
                var item = keys[i];
                if (item.ItemId == itemId)
                {
                    return item;
                }
            }

            return null;
        }

        void CheckItemKeys()
        {
            for (var i = keys.Count - 1; i >= 0; i--)
            {
                var item = keys[i];

                if (!_page.Children.Contains(item.Page))
                {
                    // Disconnect will remove the ItemKey from the keys list
                    item.Disconnect();
                }
            }
        }
    }

    class AdapterItemKey
    {
        Page _page;
        Action<AdapterItemKey>? _markInvalid;
        object? _platformView;
        bool _disconnected;

        public AdapterItemKey(Page page, Action<AdapterItemKey> markInvalid)
        {
            // We aren't setting the platform view in the ctor because
            // the PlatformView might not be valid. It might
            // be from a destroyed context or from a page that was moved
            // from a different location.
            _page = page;
            _markInvalid = markInvalid;
            _page.HandlerChanging += OnHandlerChanging;
            _page.HandlerChanged += OnHandlerChanged;
            ItemId = AView.GenerateViewId();
        }

        public bool Disconnected => _disconnected;
        public Page Page => _page;
        public long ItemId { get; }
        public void Disconnect()
        {
            _disconnected = true;
            _markInvalid?.Invoke(this);

            if (_page != null)
            {
                _page.HandlerChanging -= OnHandlerChanging;
                _page.HandlerChanged -= OnHandlerChanged;
            }

            _platformView = null;
        }

        void OnHandlerChanging(object? sender, HandlerChangingEventArgs e)
        {
            if (_platformView != null)
                Disconnect();
        }

        // This will only ever fire once. This is purely waiting
        // for the xplat view to get filled in with a PlatformView.
        // Once a handler is set, then this key is locked to that platformview. 
        // If that handler gets disconnected (OnHandlerChanging) then we have to
        // disconnect this key, and once the same page is requested again a new key/handler 
        // will need to get created. We can't reuse keys for different PlatformViews.
        // The ItemKey/PlatformView relationship is immutable
        void OnHandlerChanged(object? sender, EventArgs e)
        {
            if (_disconnected)
            {
                if (sender is Page page)
                    page.HandlerChanged -= OnHandlerChanged;

                return;
            }

            SetToStableView();
        }

        internal void SetToStableView()
        {
            _platformView = _page.Handler?.PlatformView;

            if (_platformView != null)
                _page.HandlerChanged -= OnHandlerChanged;
        }
    }
}
#endif
