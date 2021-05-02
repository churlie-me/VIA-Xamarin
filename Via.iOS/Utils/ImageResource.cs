using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Via.iOS.Utils;
using Via.Views;
using Xamarin.Forms;

[assembly: Dependency(typeof(ImageResource))]
namespace Via.iOS.Utils
{
    public class ImageResource : IImageResource
    {
        public Size GetSize(string fileName)
        {
            UIImage image = UIImage.FromFile(fileName);
            return new Size((double)image.Size.Width, (double)image.Size.Height);
        }
    }
}