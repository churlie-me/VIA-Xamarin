namespace Via.Views
{
    using Xamarin.Forms;

    #region Interfaces

    public interface IImageResource
    {
        #region Methods

        Size GetSize(string fileName);

        #endregion
    }

    #endregion
}
