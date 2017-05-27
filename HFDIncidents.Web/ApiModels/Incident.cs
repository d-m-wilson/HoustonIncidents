// HFD Incidents
// Copyright © 2016 David M. Wilson
// https://twitter.com/dmwilson_dev
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;

namespace HFDIncidents.Web.ApiModels
{
    public class Incident
    {
        public long Id { get; set; }
        public System.DateTime RetrievedDT { get; set; }
        public string Address { get; set; }
        public string CrossStreet { get; set; }
        public string KeyMap { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Longitude { get; set; }
        public bool CombinedResponse { get; set; }
        public Nullable<System.DateTime> CallTimeOpened { get; set; }
        public int AlarmLevel { get; set; }
        public int NumberOfUnits { get; set; }
        public string CurrentUnits { get; set; }
        public string Units { get; set; }
        public System.DateTime LastSeenDT { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public IncidentType IncidentType { get; set; }

        public Incident(HFDIncidents.Domain.Models.ActiveIncident ai)
        {
            Id = ai.Id;
            RetrievedDT = ai.RetrievedDT;
            Address = ai.Address;
            CrossStreet = ai.CrossStreet;
            KeyMap = ai.KeyMap;
            Latitude = ai.Latitude;
            Longitude = ai.Longitude;
            CombinedResponse = ai.CombinedResponse;
            CallTimeOpened = ai.CallTimeOpened;
            AlarmLevel = ai.AlarmLevel;
            NumberOfUnits = ai.NumberOfUnits;
            CurrentUnits = ai.CurrentUnits;
            Units = ai.Units;
            LastSeenDT = ai.LastSeenDT;
            IncidentType = new IncidentType(ai.IncidentType);
            Status = ai.Status;
            Notes = ai.Notes;
        }

        public Incident(HFDIncidents.Domain.Models.ArchivedIncident ai)
        {
            Id = ai.Id;
            RetrievedDT = ai.RetrievedDT;
            Address = ai.Address;
            CrossStreet = ai.CrossStreet;
            KeyMap = ai.KeyMap;
            Latitude = ai.Latitude;
            Longitude = ai.Longitude;
            CombinedResponse = ai.CombinedResponse;
            CallTimeOpened = ai.CallTimeOpened;
            AlarmLevel = ai.AlarmLevel;
            NumberOfUnits = ai.NumberOfUnits;
            CurrentUnits = null;
            Units = ai.Units;
            LastSeenDT = ai.LastSeenDT;
            IncidentType = new IncidentType(ai.IncidentType);
            Status = ai.Status;
            Notes = ai.Notes;
        }

    }
}