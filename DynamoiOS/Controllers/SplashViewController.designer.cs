// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace location2
{
    [Register ("SplashViewController")]
    partial class SplashViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIWebView webViewBG { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (webViewBG != null) {
                webViewBG.Dispose ();
                webViewBG = null;
            }
        }
    }
}