using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Via.ViewModels;
using Xamarin.Forms;
using Via.Models;
using Via.Helpers;
using System.Diagnostics;

namespace Via.Views
{
    public partial class AccidentModes : ContentPage
    {
        public PartyPageState partyPageState;
        public List<Item> partiesAdded;
        private ReportStatus rStatus;
        public AccidentModes(ReportStatus reportStatus)
        {
            rStatus = reportStatus;
            partiesAdded = new List<Item>(); ;
            InitializeComponent();

            try
            {
                for (int i = 0; i < Accident.availableParties.Count; i++)
                {
                    var iconFullName = Accident.availableParties[i].Icon.Substring(Accident.availableParties[i].Icon.IndexOf(" - ") + 1);

                    if (!string.IsNullOrWhiteSpace(iconFullName))
                    {
                        string[] iconName = iconFullName.Split('-');
                        Debug.WriteLine($"IconName Array before loopfindings: {iconName}, ,  {iconName.Length}");

                        if (iconName.Length == 1)
                        {
                            Accident.availableParties[i].Icon = $"https://png.icons8.com/{iconName[0]}";
                            Debug.WriteLine("Icon Name {0} and Url={1}", iconName[0], Accident.availableParties[i].Icon);
                        }
                        else
                        {
                            Accident.availableParties[i].Icon = $"https://png.icons8.com/{iconName[1]}";
                            Debug.WriteLine("Icon Name {0} and Url={1}", iconName[1], Accident.availableParties[i].Icon);
                        }
                    }
                }

                Debug.WriteLine("Accident.availableParties {0}", Accident.availableParties.Count);

                if (Accident.availableParties.Count > 0)
                {
                    accidentModes.ItemsSource = Accident.availableParties;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Accident.availableParties {0}", Accident.availableParties.Count);

                Debug.WriteLine("Exception {0}", ex.Message);
            }




        }

        private void AccidentSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            //thats all you need to make a search
            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                accidentModes.ItemsSource = Accident.availableParties;
            }
            else
            {
                accidentModes.ItemsSource = Accident.availableParties.Where(s => s.Title.ToLower().Contains(e.NewTextValue.ToLower()));
            }
        }

        private void ClosePage(object sender, EventArgs args)
        {
            Navigation.PopAsync();
        }

        private void AccidentMode_Tapped(object sender, ItemTappedEventArgs args)
        {
            var _listView = sender as ListView;
            var selectedItem = args.Item as Item;
            // set the text color of the selected item
            foreach (Item item in _listView.ItemsSource)
            {
                if (item.Equals(selectedItem))
                    if (item.IsSelected)
                    {
                        item.IsSelected = false;
                        var itemRemoved = CreateReport.selectedParties.Find(x => x.ID == item.ID);

                        //Find the index of the item being removed
                        var itemRemovedIndex = CreateReport.selectedParties.IndexOf(itemRemoved);

                        //Remove item from selectedParties list
                        CreateReport.selectedParties.Remove(itemRemoved);

                        //For parties added from the parties page
                        if (partyPageState != null)
                        {
                            partiesAdded.Remove(partiesAdded.Find(x => x.ID == item.ID));

                            partyPageState.OnPartyRemoved(itemRemovedIndex);
                        }
                    }
                    else
                    {
                        item.IsSelected = true;
                        var selectedParty = CreateReport._controls.Libary.AvailableParties.Items.Find(x => x.ID == item.ID);

                        CreateReport.selectedParties.Add(selectedParty);

                        //For parties added from the parties page
                        if (partyPageState != null)
                            partiesAdded.Add(selectedParty);
                    }

                // update listView
                item.OnPropertyChanged();
                // set the text color
                // reset text color if the items are not selected
                if (!item.IsSelected)
                {

                    item.TextColor = Color.Gray;
                    item.BackgroundColor = Color.Transparent;
                }
                else
                {

                    item.TextColor = Color.White;
                    item.BackgroundColor = Color.FromHex("#39b835");
                }
            }
        }

        private void DoneSelectingModes(object sender, EventArgs args)
        {
            if (partyPageState != null)
            {

                partyPageState.OnResumePage(partiesAdded);
                return;
            }
            else
            {
                if (CreateReport.selectedParties.Count > 0)
                    Accident.accidentPageState.OnResumePage(true);
            }
        }
    }
}
