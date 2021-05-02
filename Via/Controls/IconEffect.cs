namespace Via.Controls
{
    using Xamarin.Forms;

    public class IconImageEffect : RoutingEffect
    {
        #region Constants

        public const string GroupName = "Via";

        public const string Name = "IconImageEffect";

        #endregion

        #region Constructors

        public IconImageEffect() : base($"{GroupName}.{Name}")
        {
        }

        #endregion

        #region Properties

        public Color TintColor { get; set; }

        #endregion
    }
}
