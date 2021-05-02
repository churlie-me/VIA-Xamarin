
using System;
using System.Linq;
using CoreGraphics;
using Via.iOS.Renderers;
using Via.utils;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;


[assembly: ResolutionGroupName("Via")]
[assembly: ExportEffect(typeof(ShadowEffectRenderer), "ShadowEffect")]
namespace Via.iOS.Renderers
{
    public class ShadowEffectRenderer : PlatformEffect
    {
        public ShadowEffectRenderer()
        {
        }

        protected override void OnAttached()
        {
            try
            {

                var effect = (ShadowEffect)Element.Effects.FirstOrDefault(e => e is ShadowEffect);
                if (effect != null)
                {
                    Control.Layer.CornerRadius = effect.Radius;
                    Control.Layer.ShadowColor = effect.Color.ToCGColor();
                    Control.Layer.ShadowOffset = new CGSize(effect.DistanceX, effect.DistanceY);
                    Control.Layer.ShadowOpacity = 1.0f;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot set property on attached control. Error: {0}", ex.Message);
            }
        }

        protected override void OnDetached()
        {
        }
    }
}
