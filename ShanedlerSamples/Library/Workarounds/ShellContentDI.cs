using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShanedlerSamples
{
    public class ShellContentDI : ShellContent, IShellContentController
    {
        Page IShellContentController.GetOrCreateContent()
        {
            var page = (Page)Routing.GetOrCreateContent(this.Route, Parent.Parent.Parent.Handler.MauiContext.Services);
            Content = page;
            return page;
        }
    }
}
