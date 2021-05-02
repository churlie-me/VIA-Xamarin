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

[assembly: ExportRenderer(typeof(Dropdown), typeof(DropdownRenderer))]
namespace Via.iOS.Renderers
{
    public class DropdownRenderer : PickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);
            var view = e.NewElement as Picker;
            this.Control.BorderStyle = UITextBorderStyle.None;
        }
    }
}