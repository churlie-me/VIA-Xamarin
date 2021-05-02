using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Support.V4.Widget;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.AppCompat;
using Via.Droid.Renderers;
using Android.Support.V7.Graphics.Drawable;

[assembly: ExportRenderer(typeof(MasterDetailPage), typeof(RightMasterDetailPageRenderer))]
namespace Via.Droid.Renderers
{
    class RightMasterDetailPageRenderer : MasterDetailPageRenderer
    {
        protected override void OnElementChanged(VisualElement oldElement, VisualElement newElement)
        {
            base.OnElementChanged(oldElement, newElement);

            var fieldInfo = GetType().BaseType.GetField("_masterLayout", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var _masterLayout = (ViewGroup)fieldInfo.GetValue(this);
            var lp = new DrawerLayout.LayoutParams(_masterLayout.LayoutParameters);
            lp.Gravity = (int)GravityFlags.Right;
            _masterLayout.LayoutParameters = lp;
        }
    }
}