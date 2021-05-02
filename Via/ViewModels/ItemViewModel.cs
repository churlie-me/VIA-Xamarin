using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace Via.ViewModels
{
    public class ItemViewModel : INotifyPropertyChanged
    {
        public string Name { get; set; } = "";
        public Color TextColor { get; set; } = Color.White;
        public Color BackgroundColor { get; set; } = Color.Green;

        // event handler for updating the list views
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }
    }
}
