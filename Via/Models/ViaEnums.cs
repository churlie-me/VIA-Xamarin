using System;
using System.Collections.Generic;
using System.Text;

namespace Via.Models
{
    public enum ReportStage
    {
        Location,
        Accident,
        Parties,
        Overview
    }

    public enum FeatureInputType
    {
        Text = 0,
        Number = 1,
        SingleChoice = 2,
        MultiChoice = 3,
        Date = 4
    }

    public enum FeatureType
    {
        Accident = 0,
        Party = 1,
        Driver = 2,
        Passenger = 3,
        Victim = 4
    }

    public enum FeatureStates
    {
        AllowUnknown = 0,
        AllowInapplicable = 1,
        AllowOther = 2,
        Required = 3
    }

    public enum MenuItemType
    {
        Reports,
        Settings,
        Logout
    }

    public enum InvolvedTypes
    {
        Driver = 0,
        Passenger = 1,

    }

    public enum ReportStatus
    {
        Create = 0,
        Edit = 1
    }

    public enum VictimType
    {
        NotWounded = 0,
        Wounded = 1,
        FirstAid = 2,
        Hospitalized = 3,
        Killed = 4
    }
}
