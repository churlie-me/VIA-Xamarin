namespace Via.Controls
{
    using Xamarin.Forms;

    public class NativeCell : ViewCell
    {
        #region Fields

        public static readonly BindableProperty BackgroundColorProperty =
   BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(NativeCell), Color.FromHex("#39b835"), BindingMode.TwoWay);

        #endregion

        #region Properties

        public Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set { SetValue(BackgroundColorProperty, value); }
        }

        #endregion
    }
}
