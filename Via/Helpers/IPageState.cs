using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Via.Models;
using Xamarin.Forms;

namespace Via.Helpers
{
    public interface IPageState<T>
    {
        void OnResumePage(T o);
        void OnSleepPage();
        void OnStartPage();
    }

    public interface AccidentPageState : IPageState<bool> { }
    public interface ReportPageState : IPageState<string>
    {
        void OnNavigateToPage(ReportStage stage);
        void OnRefreshPage(ContentView view);
        void OnValidateAccidentLocation();
        string isValidLocationResponse();

    }

    public interface LocationPageState : IPageState<MediaFile> { }
    public interface ReportState : IPageState<bool> {
        void OnReportDeleted();
    }
    public interface PartyPageState : IPageState<List<Item>>
    {
        void OnPartyRemoved(int partyIndex);
        void OnPartyEdit(int partyIndex);
        void OnGridSelected(string selectedGrid);
    }
}
