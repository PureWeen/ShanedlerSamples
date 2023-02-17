using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.FixesAndWorkarounds
{
    public class ShellContentDI : ShellContent, IShellContentController
    {
        bool created = false;

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            created = false;
        }

        Page IShellContentController.GetOrCreateContent()
        {
            if (created && Content is Page createdPage)
                return createdPage;

            created = true;
            var page = (Page)Routing.GetOrCreateContent(this.Route, Parent.Parent.Parent.Handler.MauiContext.Services);
            Content = page;
            return page;
        }
    }
}
