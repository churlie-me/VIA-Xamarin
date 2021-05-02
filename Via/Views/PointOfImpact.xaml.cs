using FFImageLoading.Svg.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Via.Data;
using Via.Helpers;
using Via.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Via.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PointOfImpact : ContentPage
    {
        private Grid selectedPointOfImpact;
        private PartyItem party;
        private ImpactPoint ImpactPoint;
        private string selectedGrid;
        private PartyPageState _partyPageState;
        public PointOfImpact(PartyPageState partyPageState, PartyItem party, string selectedGrid = null)
        {
            InitializeComponent();

            try
            {
                _partyPageState = partyPageState;
                this.party = party;
                this.selectedGrid = selectedGrid;

                //Initiialise Svg Image Loading into Grid
                LoadPointOfImpact();
            }
            catch (Exception ex)
            {

            }
        }

        private void LoadPointOfImpact()
        {
            try
            {
                ImpactPoint = CreateReport._controls.Libary.ImpactPoints.Find(x => x.Grid == party.Grid);
                if (!string.IsNullOrEmpty(ImpactPoint.Grid))
                {
                    pointOfImpactSvg.Source = SvgImageSource.FromUri(new Uri(string.Format("{0}/{1}-vert.svg", Constants.ImageUrl, ImpactPoint.Grid)
                    ));
                }

                LoadGrids();
            }
            catch(Exception ex)
            {

            }
        }

        private void ClosePage(object sender, EventArgs args)
        {
            try
            {
                Navigation.RemovePage(this);
            }
            catch(Exception ex)
            {

            }
        }

        private void LoadGrids()
        {
            try
            {
                var selectionPointHolders = pointSelectionGrid.Children;
                int i = 0;
                foreach (Frame frameHolder in selectionPointHolders)
                {
                    var selectionPoints = (frameHolder.Children[0] as Grid).Children;
                    int Ids = 0;

                    var _impactPointIds = ImpactPoint.Items.ElementAt(i).Ids;
                    foreach (StackLayout stack in selectionPoints)
                    {
                        stack.ClassId = _impactPointIds.ElementAt(Ids++).ToString();

                        //For an already selected grid
                        if (!string.IsNullOrEmpty(selectedGrid))
                            if (selectedGrid == stack.ClassId)
                            {
                                stack.BackgroundColor = Color.FromRgb(216, 104, 100);
                                stack.Opacity = 0.2;
                            }
                    }

                    i++;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void OnPointOfImpactSelected(object sender, EventArgs e)
        {
            try
            {
                pointOfImpactLabel.TextColor = Color.Default;

                var selectedPointOfImpact = (sender as StackLayout);
                var selectionPointHolders = pointSelectionGrid.Children;

                if ((sender as StackLayout).BackgroundColor == Color.FromRgb(216, 104, 100))
                {
                    (sender as StackLayout).BackgroundColor = Color.Transparent;
                    (sender as StackLayout).Opacity = 1;
                    selectedGrid = "";
                }
                else
                {
                    (sender as StackLayout).BackgroundColor = Color.FromRgb(216, 104, 100);
                    (sender as StackLayout).Opacity = 0.2;

                    selectedGrid = selectedPointOfImpact.ClassId;
                }

                foreach (Frame frameHolder in selectionPointHolders)
                {
                    var selectionPoints = (frameHolder.Children[0] as Grid).Children;
                    foreach (StackLayout stack in selectionPoints)
                        if (!stack.Equals(selectedPointOfImpact))
                        {
                            stack.BackgroundColor = Color.Transparent;
                            stack.Opacity = 1;
                        }
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void POIContinue(object sender, EventArgs args)
        {
            if (!string.IsNullOrEmpty(selectedGrid))
                _partyPageState.OnGridSelected(selectedGrid);
            else
                pointOfImpactLabel.TextColor = Color.Red;
        }
    }
}