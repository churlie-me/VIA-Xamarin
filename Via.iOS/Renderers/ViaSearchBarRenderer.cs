using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using CoreGraphics;
using System.ComponentModel;
using Via.iOS.Renderers;
using Via.Controls;
using CoreAnimation;

[assembly: ExportRenderer(typeof(ViaSearchBar), typeof(ViaSearchBarRenderer))]
namespace Via.iOS.Renderers
{
    public class ViaSearchBarRenderer : SearchBarRenderer
    {
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Control != null)
            {
                Control.ShowsCancelButton = false;

                UITextField txSearchField = (UITextField)Control.ValueForKey(new Foundation.NSString("searchField"));
                txSearchField.BackgroundColor = UIColor.White;
                txSearchField.BorderStyle = UITextBorderStyle.None;
                txSearchField.Layer.BorderWidth = 1.0f;
                txSearchField.Layer.CornerRadius = 2.0f;
                txSearchField.Layer.BorderColor = UIColor.LightGray.CGColor;

            }
        }
    }
}