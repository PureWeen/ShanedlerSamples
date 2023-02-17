#if IOS || MACCATALYST
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace Maui.FixesAndWorkarounds.Library.Common
{
    internal static partial class Extensions
    {
        internal static T? FindResponder<T>(this UIView view) where T : UIResponder
        {
            var nextResponder = view as UIResponder;
            while (nextResponder is not null)
            {
                nextResponder = nextResponder.NextResponder;

                if (nextResponder is T responder)
                    return responder;
            }
            return null;
        }

        internal static UIView? FindNextView(this UIView? view, UIView containerView, Func<UIView, bool> isValidType)
        {
            UIView? nextView = null;

            while (view is not null && view != containerView && nextView is null)
            {
                var siblings = view.Superview?.Subviews;

                if (siblings is null)
                    break;

                nextView = view.FindNextView(siblings.IndexOf(view) + 1, isValidType);

                view = view.Superview;
            }

            // if we did not find the next view, try to find the first one
            nextView ??= containerView.Subviews?[0]?.FindNextView(0, isValidType);

            return nextView;
        }

        static UIView? FindNextView(this UIView? view, int index, Func<UIView, bool> isValidType)
        {
            // search through the view's siblings and traverse down their branches
            var siblings = view?.Superview?.Subviews;

            if (siblings is null)
                return null;

            for (int i = index; i < siblings.Length; i++)
            {
                var sibling = siblings[i];

                if (sibling.Subviews is not null && sibling.Subviews.Length > 0)
                {
                    var childVal = sibling.Subviews[0].FindNextView(0, isValidType);
                    if (childVal is not null)
                        return childVal;
                }

                if (isValidType(sibling))
                    return sibling;
            }

            return null;
        }

        internal static void ChangeFocusedView(this UIView view, UIView? newView)
        {
            if (newView is null)
                view.ResignFirstResponder();

            else
                newView.BecomeFirstResponder();
        }
    }
}
#endif