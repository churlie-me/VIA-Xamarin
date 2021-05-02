using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Via.Helpers
{
    [ContentProperty("Source")]
    class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }
        public string cd = "Resources/";

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
            {
                return null;
            }
            // Do your translation lookup here, using whatever method you require
            var imageSource = Device.RuntimePlatform == Device.iOS ? ImageSource.FromFile(Source) : ImageSource.FromFile(cd + Source);

            return imageSource;
        }
    }
}
