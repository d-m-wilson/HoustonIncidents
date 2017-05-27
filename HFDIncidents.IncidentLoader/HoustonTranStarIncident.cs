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

namespace HFDIncidents.IncidentLoader
{
    [Serializable]
    public class HoustonTranStarIncident
    {
        public string location { get; set; }
        public string desc { get; set; }
        public string veh { get; set; }
        public string lanes { get; set; }
        public string status { get; set; }
        public string time { get; set; }
        public string date { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }

        public byte NumberOfVehicles
        {
            get
            {
                if (!initialized) Initialize();
                return numberOfVehicles;
            }
        }

        public DateTime DateTime
        {
            get
            {
                if (!initialized) Initialize();
                return dateTime;
            }
        }

        private volatile bool initialized = false;
        private byte numberOfVehicles;
        private DateTime dateTime;
        private readonly object lockObject = new object();

        private void Initialize()
        {
            lock (lockObject)
            {
                if (initialized) return;
                initialized = true;

                if (!byte.TryParse(veh, out numberOfVehicles))
                {
                    numberOfVehicles = 0;
                }

                if (date == "yesterday")
                {
                    dateTime = DateTime.Today.AddDays(-1);
                }
                else if (date == "today" || !DateTime.TryParse(date, out dateTime))
                {
                    dateTime = DateTime.Today;
                }

                DateTime tempTime;
                DateTime.TryParse(time, out tempTime);

                DateTime combinedDT = new DateTime(
                        dateTime.Year,
                        dateTime.Month,
                        dateTime.Day,
                        tempTime.Hour,
                        tempTime.Minute,
                        tempTime.Second,
                        DateTimeKind.Local);

                dateTime = combinedDT;
            }
        }
    }

}
