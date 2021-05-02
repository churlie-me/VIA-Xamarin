
using Android.Graphics;
using Android.Widget;
using Java.IO;
using Via.Droid.Utils;
using Xamarin.Forms;

[assembly: Dependency(typeof(ImageResource))]
namespace Via.Droid.Utils
{
    class ImageResource : Java.Lang.Object, Via.Views.IImageResource
    {
        public Size GetSize(string fileName)
        {
            var options = new BitmapFactory.Options
            {
                InJustDecodeBounds = true
            };

            //fileName = fileName.Replace('-', '_').Replace(".png", "").Replace(".jpg", "");
            //var resId = Forms.Context.Resources.GetIdentifier(fileName, "drawable", Forms.Context.PackageName);
            //BitmapFactory.DecodeResource(Forms.Context.Resources, resId, options);
            BitmapFactory.DecodeFile(fileName, options);
          

            return new Size((double)options.OutWidth, (double)options.OutHeight);
        }
    }
}