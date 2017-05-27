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

namespace HFDIncidents.Domain.Models
{
    public partial class ActiveIncident
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
        public Nullable<long> IncidentTypeId { get; set; }
        public int AlarmLevel { get; set; }
        public int NumberOfUnits { get; set; }
        public string CurrentUnits { get; set; }
        public string Units { get; set; }
        public System.DateTime LastSeenDT { get; set; }
        public long ServiceSessionId { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public virtual IncidentType IncidentType { get; set; }
    }
}
