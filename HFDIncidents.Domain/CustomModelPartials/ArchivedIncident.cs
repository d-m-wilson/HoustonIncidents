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

namespace HFDIncidents.Domain.Models
{
    public partial class ArchivedIncident
    {
        public static ArchivedIncident CreateFromActiveIncident(ActiveIncident src)
        {
            var dst = new ArchivedIncident
            {
                RetrievedDT = src.RetrievedDT,
                Address = src.Address,
                CrossStreet = src.CrossStreet,
                KeyMap = src.KeyMap,
                Latitude = src.Latitude,
                Longitude = src.Longitude,
                CombinedResponse = src.CombinedResponse,
                CallTimeOpened = src.CallTimeOpened,
                IncidentType = src.IncidentType,
                AlarmLevel = src.AlarmLevel,
                Units = src.Units,
                LastSeenDT = src.LastSeenDT,
                Status = src.Status,
                Notes = src.Notes,
            };

            try
            {
                if (!string.IsNullOrWhiteSpace(src.Units) && src.Units.Contains(";"))
                {
                    dst.NumberOfUnits = src.Units.Split(new [] { ';' }, System.StringSplitOptions.RemoveEmptyEntries).Length;
                }
                else
                {
                    dst.NumberOfUnits = src.NumberOfUnits;
                }
            }
            catch
            {
                dst.NumberOfUnits = src.NumberOfUnits;
            }

            return dst;
        }
    }
}
