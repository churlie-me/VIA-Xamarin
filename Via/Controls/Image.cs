namespace Via.Controls
{
    using Xamarin.Forms;

    public class Image : Xamarin.Forms.Image
    {
        #region Fields
        public static readonly BindableProperty TintColorProperty = BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(Image), Color.Gray, BindingMode.TwoWay);
        #endregion
 

        #region Properties

        public Color TintColor
        {
            get
            {
                return (Color)GetValue(TintColorProperty);
            }
            set
            {
                SetValue(TintColorProperty, value);
            }
        }

       

        #endregion
    }
}
