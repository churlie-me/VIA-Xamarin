using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using SQLite;

namespace Via.Models
{
    public class MenuItem
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
    }

    public class MainMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Color { get; set; }

        public string Title { get; set; }

        public bool ShowBorder { get; set; } = false;
    }

    public class ChangePassword
    {
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
        public string newPasswordCheck { get; set; }
    }

    public class Result
    {
        //map json model
        public string alternative_names { get; set; }
        public List<double> boundingbox { get; set; }
        public string display_name { get; set; }      
        public double lon { get; set; }
        public double lat { get; set; }
        public string name { get; set; }
        public string name_suffix { get; set; }
        public string type { get; set; }
    }

    public class SearchLocation
    {
        public int count { get; set; }
        public int nextIndex { get; set; }
        public int startIndex { get; set; }
        public int totalResults { get; set; }
        public List<Result> results { get; set; }
    }

    public class ViaUser
    {
        private string userID { get; set; }
        public string username { get; set; }
        public Token token { get; set; }
        public string status { get; set; }
        public Profile profile { get; set; }
        public Settings settings { get; set; }
        public InitialSettings InitialSettings { get; set; }
    }


    public class Token
    {
        public string token { get; set; }
        public string refreshToken { get; set; }
        public string grantType { get; set; }
        public string validTo { get; set; }
    }

    public class Profile
    {
        public string gender { get; set; }
        public string firstname { get; set; }
        public string middlename { get; set; }
        public string lastname { get; set; }
        public string fullname { get; set; }
        public string function { get; set; }
        public string avatar { get; set; }
    }

    public class Avatar
    {
        public MediaFile image { get; set; }
        public int x { get; set; }
        public int Y { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

   

    public class Settings
    {
        public int mainAreaID { get; set; }
        public string timeZone { get; set; }
        public string culture { get; set; }
        public string speedUnit { get; set; }
        public string mailInterval { get; set; }
        public string mailDayOfWeek { get; set; }
    }

    public class PasswordRules
    {
        public int minimalCharacters { get; set; }
        public bool needsUpperCaseLetter { get; set; }
        public bool needsLowerCaseLetter { get; set; }
        public bool needsDecimalDigit { get; set; }
    }

    public class InitialSettings
    {
        public PasswordRules passwordRules { get; set; }
        public List<string> mandatoryFields { get; set; }
        public List<string> speedUnits { get; set; }
        public List<string> genders { get; set; }
        public List<string> mailIntervals { get; set; }
    }

    public class ResponseMessage
    {
        public string message { get; set; }
    }

    public class Credentials
    {
        public string username { get; set; }
        public string password { get; set; }
        public string grantType { get; set; }
    }

    public class ForgotCredentials
    {
        public string emailaddress { get; set; }
        public string callbackUrl { get; set; }
    }

    public class AuthorizeRefresh
    {
        public string refreshToken { get; set; }
        public string grantType { get; set; }
    }

    public class AccidentHistory
    {
        public AccidentData data { get; set; }
        public int draw { get; set; }
        public int recordsFiltered { get; set; }
        public int recordsTotal { get; set; }
    }

    public class AccidentRequest
    {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public int ordercolumn { get; set; }
        public string orderdir { get; set; }
        public string search { get; set; }
        public bool clearCache { get; set; }
    }

    public class AccidentData   
    {
        public string code { get; set; }
        public DateTime datum { get; set; }
        public string omschrijving { get; set; }
        public string pvnr { get; set; }
    }

    public class AccidentGraphic
    {
        public string Url { get; set; }
    }

    //Accident Report
    public class DataFeature
    {
        public string Id { get; set; }
        public List<string> Values { get; set; }
        public string Other { get; set; }
    }

    public class LocationAttribute
    {
        public string Field { get; set; }
        public string Value { get; set; }
    }

    public class Data
    {
        public bool ChangedLocation { get; set; }
        public long DateTime { get; set; } //Time in milliseconds
        public List<ReportFeature> Features { get; set; }
        public long GenerationDateTime { get; set; } //Time in milliseconds
        public Int64 Id { get; set; }
        public List<ReportImage> Images { get; set; }
        public string LND { get; set; }
        public List<LocationAttribute> LocationAttributes { get; set; }
        public long MutationDateTime { get; set; } //Time in milliseconds
        public List<Party> Parties { get; set; }
    }

    public class Item : INotifyPropertyChanged
    {
        public List<ExcludeFeature> ExcludeFeatureIds { get; set; }
        public string ID { get; set; }
        public string Title { get; set; }
        public string Grid { get; set; }
        public string Icon { get; set; }
        public ImageSource IconSource { get; set; }
        public List<string> InvolvedTypes { get; set; }
        public object Tags { get; set; }

        public Color TextColor { get; set; } = Color.Gray;
        public Color BackgroundColor { get; set; } = Color.Transparent;

        //Keep track of each item selected
        public bool IsSelected { get; set; } = false;


        //
        public string IconCollection { get; set; }

        // event handler for updating the list views
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }
    }

    public class PartyItem : INotifyPropertyChanged
    {
        public List<ExcludeFeature> ExcludeFeatureIds { get; set; }
        public string ID { get; set; }
        public string Title { get; set; }
        public string Grid { get; set; }
        public string IsValid { get; set; } = "";
        public string Icon { get; set; }
        public List<string> InvolvedTypes { get; set; }
        public object Tags { get; set; }

        public Color TextColor { get; set; } = Color.White;
        public Color BackgroundColor { get; set; } = Color.FromHex("#6a6a77");

        //Keep track of each item selected
        public bool IsSelected { get; set; } = false;


        //
        public string IconCollection
        {
            get => Icon + "/" + TextColor;
            set => IconCollection = value;
        }

        // event handler for updating the list views
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }
    }

    public class ExcludeFeature
    {
        public string FeatureId { get; set; }
	    public List<object> ItemId { get; set; }
    }

    public class AvailableParties
    {
        public List<object> ExcludeFeatureIds { get; set; }
        public string FeatureInputType { get; set; }
        public List<object> Featur0eStates { get; set; }
        public List<string> FeatureTypes { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public List<Item> Items { get; set; }
    }

    public class FeatureStateValue
    {
        public string FeatureState { get; set; }
        public string Value { get; set; }
    }

    public class FeatureItem
    {
        public List<ExcludeFeature> ExcludeFeatureIds { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string __type { get; set; }
        public List<object> NotCombinedWithItemIds { get; set; }
    }

    public class Feature
    {
        public string __type { get; set; }
        public List<object> ExcludeFeatureIds { get; set; }
        public string FeatureInputType { get; set; }
        public string [] FeatureStates { get; set; }
        public List<string> FeatureTypes { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public List<FeatureItem> Items { get; set; }
        public int? Decimals { get; set; }
        public string Format { get; set; }
        public int? MaxValue { get; set; }
        public int? MinValue { get; set; }
        public string FormatValidator { get; set; }
        public int? MaxLenth { get; set; }
        public int? MaxSelectedItems { get; set; }
        public long? End { get; set; }
        public int? Start { get; set; }
    }

    public class ImpactPointItem
    {
        public List<int> Columns { get; set; }
        public int Height { get; set; }
        public List<string> Ids { get; set; }
    }

    public class ImpactPoint
    {
        public string Grid { get; set; }
        public List<ImpactPointItem> Items { get; set; }
    }

    public class IntendedMovement
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }

    public class PrecedingMovement
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }

    public class Mode
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int Grid { get; set; }
        public string Icon { get; set; }
        public List<string> InvolvedTypes { get; set; }
    }

    public class Libary
    {
        public AvailableParties AvailableParties { get; set; }
        public List<FeatureStateValue> FeatureStateValues { get; set; }
        public List<Feature> Features { get; set; }
        public int ImpactPointId { get; set; }
        public List<ImpactPoint> ImpactPoints { get; set; }
        public List<IntendedMovement> IntendedMovements { get; set; }
        public List<PrecedingMovement> PrecedingMovements { get; set; }
        public string Version { get; set; }
    }

    public class Control
    {
        public Data Data { get; set; }
        public object ErrorMessage { get; set; }
        public Libary Libary { get; set; }
    }

    public class ReportData
    {
        public Data Data { get; set; }
    }

    public class ReportFeature
    {
        public string Id { get; set; }
        public List<string> Values { get; set; }
        public string Other { get; set; } = null;
    }

    public class Party
    {
        public string Id { get; set; }
        public List<ReportFeature> Features { get; set; }
        public List<Person> Persons { get; set; }
    }

    public class Cultures
    {
        [JsonProperty("en-GB")] public string en_GB { get; set; }

        [JsonProperty("fr-FR")] public string fr_FR { get; set; }

        [JsonProperty("nl-NL")] public string nl_NL { get; set; }
    }

    public class Person
    {
        public string Id { get; set; }
        public string DateOfDeath { get; set; }
        public List<ReportFeature> Features { get; set; }
        public string Type { get; set; }
        public string VictimType { get; set; }
    }

    public class Driver
    {
        public string guiltyStatus { get; set; }
        public int entryIntoCirculation { get; set; }
        public int numberOfPermits { get; set; }
        public string yearOfLicence { get; set; }
        public DateTime dateOfBirth { get; set; }
        public string Gender { get; set; }
    }

    public class Passenger
    {
        public DateTime dateOfBirth { get; set; }
        public string Gender { get; set; }
    }

    public class PartyView
    {
        public ContentView ContentView { get; set; }
        public int passengers { get; set; }
        public bool isValid { get; set; }
        public string selectedGrid { get; set; }
    }

    public class StoredReport
    {
        public string ID { get; set; }
        public string Message { get; set; }
        public long MutationDateTime { get; set; }
        public string QRCodeBase64 { get; set; }
        public string Result { get; set; }
    }

    public class ScrollableDate : INotifyPropertyChanged
    {
        public string Day { get; set; }
        public int Date { get; set; }
        public string Month { get; set; }

        public Color ItemTopBackgroundColor { get; set; } = Color.White;
        public Color ItemSeparatorColor { get; set; } = Color.FromHex("#0000ffff");
        public Color ItemBottomBackgroundColor { get; set; } = Color.White;
        public Color ItemTextColor { get; set; } = Color.Default;

        //Keep track of each item selected
        public bool IsSelected { get; set; } = false;

        public int clickCount { get; set; } = 0;

        // event handler for updating the list views
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }
    }

    public class ScrollableTime : INotifyPropertyChanged
    {
        public string Time { get; set; }
        public string ItemIcon { get; set; } = "ic_clock.png";
        public Color ItemTopBackgroundColor { get; set; } = Color.White;
        public Color ItemSeparatorColor { get; set; } = Color.FromHex("#0000ffff");
        public Color ItemBottomBackgroundColor { get; set; } = Color.White;
        public Color ItemTextColor { get; set; } = Color.Default;

        //Keep track of each item selected
        public bool IsSelected { get; set; } = false;

        public int clickCount { get; set; } = 0;

        // event handler for updating the list views
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }
    }

    public class ContactArea
    {
        public string ErrorMessage { get; set; }
        public Extent extent { get; set; }
        public List<string> LND { get; set; }
    }

    public class Extent
    {
        public double maxX { get; set; }
        public double maxY { get; set; }
        public double minX { get; set; }
        public double minY { get; set; }
    }

    public class SqlLiteReport
    {
        [PrimaryKey, AutoIncrement]
        public int reportID { get; set; }
        public string reportData { get; set; }
    }

    public class TimeZones
    {
        [JsonProperty("Dateline Standard Time")]
        public string DatelineStandardTime { get; set; }

        [JsonProperty("UTC-11")] public string UTC_11 { get; set; }

        [JsonProperty("Aleutian Standard Time")]
        public string AleutianStandardTime { get; set; }

        [JsonProperty("Hawaiian Standard Time")]
        public string Hawaiian_StandardTime { get; set; }

        [JsonProperty("Marquesas Standard Time")]
        public string MarquesasStandardTime { get; set; }

        [JsonProperty("Alaskan Standard Time")]
        public string AlaskanStandardTime { get; set; }

        [JsonProperty("UTC-09")] public string UTC_09 { get; set; }

        [JsonProperty("Pacific Standard Time (Mexico)")]
        public string PacificStandardTimeMexico { get; set; }

        [JsonProperty("UTC-08")] public string UTC_08 { get; set; }

        [JsonProperty("Pacific Standard Time")]
        public string PacificStandardTime { get; set; }

        [JsonProperty("US Mountain Standard Time")]
        public string USMountainStandardTime { get; set; }

        [JsonProperty("Mountain Standard Time (Mexico)")]
        public string MountainStandardTimeMexico { get; set; }

        [JsonProperty("Mountain Standard Time")]
        public string MountainStandardTime { get; set; }

        [JsonProperty("Central America Standard Time")]
        public string CentralAmericaStandardTime { get; set; }

        [JsonProperty("Central Standard Time")]
        public string CentralStandardTime { get; set; }

        [JsonProperty("Easter Island Standard Time")]
        public string EasterIslandStandardTime { get; set; }

        [JsonProperty("Central Standard Time (Mexico)")]
        public string CentralStandardTimeMexico { get; set; }

        [JsonProperty("Canada Central Standard Time")]
        public string CanadaCentralStandardTime { get; set; }

        [JsonProperty("SA Pacific Standard Time")]
        public string SAPacificStandardTime { get; set; }

        [JsonProperty("Eastern Standard Time (Mexico)")]
        public string EasternStandardTimeMexico { get; set; }

        [JsonProperty("Eastern Standard Time")]
        public string EasternStandardTime { get; set; }

        [JsonProperty("Haiti Standard Time")] public string HaitiStandardTime { get; set; }

        [JsonProperty("Cuba Standard Time")] public string CubaStandardTime { get; set; }

        [JsonProperty("US Eastern Standard Time")]
        public string USEasternStandardTime { get; set; }

        [JsonProperty("Turks And Caicos Standard Time")]
        public string TurksAndCaicosStandardTime { get; set; }

        [JsonProperty("Paraguay Standard Time")]
        public string ParaguayStandardTime { get; set; }

        [JsonProperty("Atlantic Standard Time")]
        public string AtlanticStandardTime { get; set; }

        [JsonProperty("Venezuela Standard Time")]
        public string VenezuelaStandardTime { get; set; }

        [JsonProperty("Central Brazilian Standard Time")]
        public string CentralBrazilianStandardTime { get; set; }

        [JsonProperty("SA Western Standard Time")]
        public string SAWesternStandardTime { get; set; }

        [JsonProperty("Pacific SA Standard Time")]
        public string PacificSAStandardTime { get; set; }

        [JsonProperty("Newfoundland Standard Time")]
        public string NewfoundlandStandardTime { get; set; }

        [JsonProperty("Tocantins Standard Time")]
        public string TocantinsStandardTime { get; set; }

        [JsonProperty("E. South America Standard Time")]
        public string E_SouthAmericaStandardTime { get; set; }

        [JsonProperty("SA Eastern Standard Time")]
        public string SAEasternStandardTime { get; set; }

        [JsonProperty("Argentina Standard Time")]
        public string ArgentinaStandardTime { get; set; }

        [JsonProperty("Greenland Standard Time")]
        public string GreenlandStandardTime { get; set; }

        [JsonProperty("Montevideo Standard Time")]
        public string MontevideoStandardTime { get; set; }

        [JsonProperty("Magallanes Standard Time")]
        public string MagallanesStandardTime { get; set; }

        [JsonProperty("Saint Pierre Standard Time")]
        public string SaintPierreStandardTime { get; set; }

        [JsonProperty("Bahia Standard Time")] public string BahiaStandardTime { get; set; }

        [JsonProperty("UTC-02")] public string UTC_02 { get; set; }

        [JsonProperty("Mid-Atlantic Standard Time")]
        public string Mid_Atlantic_StandardTime { get; set; }

        [JsonProperty("Azores Standard Time")] public string AzoresStandardTime { get; set; }

        [JsonProperty("Cape Verde Standard Time")]
        public string CapeVerdeStandardTime { get; set; }

        [JsonProperty("UTC")] public string UTC { get; set; }

        [JsonProperty("GMT Standard Time")] public string GMTStandardTime { get; set; }

        [JsonProperty("Greenwich Standard Time")]
        public string GreenwichStandardTime { get; set; }

        [JsonProperty("W. Europe Standard Time")]
        public string W_EuropeStandardTime { get; set; }

        [JsonProperty("Central Europe Standard Time")]
        public string CentralEuropeStandardTime { get; set; }

        [JsonProperty("Romance Standard Time")]
        public string Romance_StandardTime { get; set; }

        [JsonProperty("Morocco Standard Time")]
        public string MoroccoStandardTime { get; set; }

        [JsonProperty("Sao Tome Standard Time")]
        public string SaoTomeStandardTime { get; set; }

        [JsonProperty("Central European Standard Time")]
        public string CentralEuropeanStandardTime { get; set; }

        [JsonProperty("W. Central Africa Standard Time")]
        public string W_CentralAfricaStandardTime { get; set; }

        [JsonProperty("Jordan Standard Time")] public string JordanStandardTime { get; set; }

        [JsonProperty("GTB Standard Time")] public string GTBStandardTime { get; set; }

        [JsonProperty("Middle East Standard Time")]
        public string MiddleEastStandardTime { get; set; }

        [JsonProperty("Egypt Standard Time")] public string EgyptStandardTime { get; set; }

        [JsonProperty("E. Europe Standard Time")]
        public string E_EuropeStandardTime { get; set; }

        [JsonProperty("Syria Standard Time")] public string SyriaStandardTime { get; set; }

        [JsonProperty("West Bank Standard Time")]
        public string WestBankStandardTime { get; set; }

        [JsonProperty("South Africa Standard Time")]
        public string SouthAfricaStandardTime { get; set; }

        [JsonProperty("FLE Standard Time")] public string FLE_StandardTime { get; set; }

        [JsonProperty("Israel Standard Time")] public string Israel_StandardTime { get; set; }

        [JsonProperty("Kaliningrad Standard Time")]
        public string KaliningradStandardTime { get; set; }

        [JsonProperty("Sudan Standard Time")] public string SudanStandardTime { get; set; }

        [JsonProperty("Libya Standard Time")] public string LibyaStandardTime { get; set; }

        [JsonProperty("Namibia Standard Time")]
        public string NamibiaStandardTime { get; set; }

        [JsonProperty("Arabic Standard Time")] public string ArabicStandardTime { get; set; }

        [JsonProperty("Turkey Standard Time")] public string TurkeyStandardTime { get; set; }

        [JsonProperty("Arab Standard Time")] public string ArabStandardTime { get; set; }

        [JsonProperty("Belarus Standard Time")]
        public string BelarusStandardTime { get; set; }

        [JsonProperty("Russian Standard Time")]
        public string RussianStandardTime { get; set; }

        [JsonProperty("E. Africa Standard Time")]
        public string E_AfricaStandardTime { get; set; }

        [JsonProperty("Iran Standard Time")] public string IranStandardTime { get; set; }

        [JsonProperty("Arabian Standard Time")]
        public string ArabianStandardTime { get; set; }

        [JsonProperty("Astrakhan Standard Time")]
        public string AstrakhanStandardTime { get; set; }

        [JsonProperty("Azerbaijan Standard Time")]
        public string AzerbaijanStandardTime { get; set; }

        [JsonProperty("Russia Time Zone 3")] public string RussiaTimeZone3 { get; set; }

        [JsonProperty("Mauritius Standard Time")]
        public string MauritiusStandardTime { get; set; }

        [JsonProperty("Saratov Standard Time")]
        public string SaratovStandardTime { get; set; }

        [JsonProperty("Georgian Standard Time")]
        public string GeorgianStandardTime { get; set; }

        [JsonProperty("Volgograd Standard Time")]
        public string VolgogradStandardTime { get; set; }

        [JsonProperty("Caucasus Standard Time")]
        public string CaucasusStandardTime { get; set; }

        [JsonProperty("Afghanistan Standard Time")]
        public string AfghanistanStandardTime { get; set; }

        [JsonProperty("West Asia Standard Time")]
        public string WestAsiaStandardTime { get; set; }

        [JsonProperty("Ekaterinburg Standard Time")]
        public string EkaterinburgStandardTime { get; set; }

        [JsonProperty("Pakistan Standard Time")]
        public string PakistanStandardTime { get; set; }

        [JsonProperty("India Standard Time")] public string IndiaStandardTime { get; set; }

        [JsonProperty("Sri Lanka Standard Time")]
        public string SriLankaStandardTime { get; set; }

        [JsonProperty("Nepal Standard Time")] public string NepalStandardTime { get; set; }

        [JsonProperty("Central Asia Standard Time")]
        public string CentralAsiaStandardTime { get; set; }

        [JsonProperty("Bangladesh Standard Time")]
        public string BangladeshStandardTime { get; set; }

        [JsonProperty("Omsk Standard Time")] public string OmskStandardTime { get; set; }

        [JsonProperty("Myanmar Standard Time")]
        public string MyanmarStandardTime { get; set; }

        [JsonProperty("SE Asia Standard Time")]
        public string SEAsiaStandardTime { get; set; }

        [JsonProperty("Altai Standard Time")] public string AltaiStandardTime { get; set; }

        [JsonProperty("W. Mongolia Standard Time")]
        public string W_MongoliaStandardTime { get; set; }

        [JsonProperty("North Asia Standard Time")]
        public string NorthAsiaStandardTime { get; set; }

        [JsonProperty("N. Central Asia Standard Time")]
        public string N_CentralAsiaStandardTime { get; set; }

        [JsonProperty("Tomsk Standard Time")] public string TomskStandardTime { get; set; }

        [JsonProperty("China Standard Time")] public string ChinaStandardTime { get; set; }

        [JsonProperty("North Asia East Standard Time")]
        public string NorthAsiaEastStandardTime { get; set; }

        [JsonProperty("Singapore Standard Time")]
        public string SingaporeStandardTime { get; set; }

        [JsonProperty("W. Australia Standard Time")]
        public string W_AustraliaStandardTime { get; set; }

        [JsonProperty("Taipei Standard Time")] public string TaipeiStandardTime { get; set; }

        [JsonProperty("Ulaanbaatar Standard Time")]
        public string UlaanbaatarStandardTime { get; set; }

        [JsonProperty("Aus Central W. Standard Time")]
        public string AusCentral_W_StandardTime { get; set; }

        [JsonProperty("Transbaikal Standard Time")]
        public string TransbaikalStandardTime { get; set; }

        [JsonProperty("Tokyo Standard Time")] public string TokyoStandardTime { get; set; }

        [JsonProperty("North Korea Standard Time")]
        public string NorthKoreaStandardTime { get; set; }

        [JsonProperty("Korea Standard Time")] public string KoreaStandardTime { get; set; }

        [JsonProperty("Yakutsk Standard Time")]
        public string YakutskStandardTime { get; set; }

        [JsonProperty("Cen. Australia Standard Time")]
        public string CenAustraliaStandardTime { get; set; }

        [JsonProperty("AUS Central Standard Time")]
        public string AUSCentralStandardTime { get; set; }

        [JsonProperty("E. Australia Standard Time")]
        public string E_AustraliaStandardTime { get; set; }

        [JsonProperty("AUS Eastern Standard Time")]
        public string AUSEasternStandardTime { get; set; }

        [JsonProperty("West Pacific Standard Time")]
        public string WestPacificStandardTime { get; set; }

        [JsonProperty("Tasmania Standard Time")]
        public string TasmaniaStandardTime { get; set; }

        [JsonProperty("Vladivostok Standard Time")]
        public string VladivostokStandardTime { get; set; }

        [JsonProperty("Lord Howe Standard Time")]
        public string LordHoweStandardTime { get; set; }

        [JsonProperty("Bougainville Standard Time")]
        public string BougainvilleStandardTime { get; set; }

        [JsonProperty("Russia Time Zone 10")] public string RussiaTimeZone_10 { get; set; }

        [JsonProperty("Magadan Standard Time")]
        public string MagadanStandardTime { get; set; }

        [JsonProperty("Norfolk Standard Time")]
        public string NorfolkStandardTime { get; set; }

        [JsonProperty("Sakhalin Standard Time")]
        public string SakhalinStandardTime { get; set; }

        [JsonProperty("Central Pacific Standard Time")]
        public string CentralPacificStandardTime { get; set; }

        [JsonProperty("Russia Time Zone 11")] public string RussiaTimeZone11 { get; set; }

        [JsonProperty("New Zealand Standard Time")]
        public string NewZealandStandardTime { get; set; }

        [JsonProperty("UTC+12")] public string UTC_12 { get; set; }

        [JsonProperty("Fiji Standard Time")] public string FijiStandardTime { get; set; }

        [JsonProperty("Kamchatka Standard Time")]
        public string KamchatkaStandardTime { get; set; }

        [JsonProperty("Chatham Islands Standard Time")]
        public string ChathamIslandsStandardTime { get; set; }

        [JsonProperty("UTC+13")] public string UTC_13 { get; set; }

        [JsonProperty("Tonga Standard Time")] public string TongaStandardTime { get; set; }

        [JsonProperty("Samoa Standard Time")] public string SamoaStandardTime { get; set; }

        [JsonProperty("Line Islands Standard Time")]
        public string LineIslandsStandardTime { get; set; }
    }

    //Returned Image Object after uploading
    public class ReportImage
    {
        public string Id { get; set; }
        public string Url { get; set; }
        //public ImageSource ImageSource { get; set; } = null;
    }

    public class ContactAreaItem : INotifyPropertyChanged
    {
        public string Country { get; set; }
        public Color BackgroundColor { get; set; } = Color.Transparent;
        public Color BorderColor { get; set; } = Color.FromHex("#d5dde1");

        //Keep track of each item selected
        public bool IsSelected { get; set; } = false;

        // event handler for updating the list views
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }
    }

    public class Report
    {
        public string ID { get; set; }
        public List<ReportImage> Images { get; set; }
        public DateTime DateTime { get; set; }
        public string Location { get; set; }
    }
}