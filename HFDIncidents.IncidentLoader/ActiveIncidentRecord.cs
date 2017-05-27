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
    public class ActiveIncidentRecord
    {
        [NonSerialized]
        private readonly DateTime _dtRetrieved;
        [NonSerialized]
        private string _strCombinedResponse;
        [NonSerialized]
        private string _strCallTimeOpened;
        [NonSerialized]
        private DateTime _dtCallTimeOpened;
        [NonSerialized]
        private string _strAlarmLevel;
        [NonSerialized]
        private int _nAlarmLevel;
        [NonSerialized]
        private string _strNumberOfUnits;
        [NonSerialized]
        private int _nNumberOfUnits;

        public ActiveIncidentRecord()
        {
            _dtRetrieved = DateTime.Now;
        }

        public string Agency { get; set; }
        public string Address { get; set; }
        public string CrossStreet { get; set; }
        public string KeyMap { get; set; }
        public string XCoord { get; set; }
        public string YCoord { get; set; }
        public double Latitude
        {
            get { return ParseCoordinate(YCoord); }
        }
        public double Longitude
        {
            get { return ParseCoordinate(XCoord); }
        }
        public string CombinedResponse
        {
            get
            {
                return String.IsNullOrWhiteSpace(_strCombinedResponse) ? "N" : _strCombinedResponse;
            }
            set
            {
                _strCombinedResponse = value;
            }
        }
        public string CallTimeOpened
        {
            get
            {
                return _strCallTimeOpened;
            }
            set
            {
                DateTime dtNewValue;
                _strCallTimeOpened = value;
                if (DateTime.TryParse(value, out dtNewValue))
                {
                    _dtCallTimeOpened = dtNewValue;
                }
                else
                {
                    _dtCallTimeOpened = DateTime.MinValue;
                }
            }
        }
        public DateTime CallTimeOpenedDT
        {
            get
            {
                return _dtCallTimeOpened;
            }
        }
        public string IncidentType { get; set; }
        public string AlarmLevel
        {
            get
            {
                return _strAlarmLevel;
            }
            set
            {
                int nNewValue;
                _strAlarmLevel = value;
                if (int.TryParse(value, out nNewValue))
                {
                    _nAlarmLevel = nNewValue;
                }
                else
                {
                    _nAlarmLevel = 0;
                }
            }
        }
        public int AlarmLevelInt
        {
            get
            {
                return _nAlarmLevel;
            }
        }
        public string NumberOfUnits
        {
            get
            {
                return _strNumberOfUnits;
            }
            set
            {
                int nNewValue;
                _strNumberOfUnits = value;
                if (int.TryParse(value, out nNewValue))
                {
                    _nNumberOfUnits = nNewValue;
                }
                else
                {
                    _nNumberOfUnits = 1;
                }
            }
        }
        public int NumberOfUnitsInt
        {
            get
            {
                return _nNumberOfUnits;
            }
        }
        public string Units { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }

        public DateTime DateRetrieved { get { return _dtRetrieved; } }

        private double ParseCoordinate(string strCoordinate)
        {
            try
            {
                int insertAt;

                if (strCoordinate[0] == '-')
                {
                    insertAt = 3;
                }
                else
                {
                    insertAt = 2;
                }

                strCoordinate = strCoordinate.Insert(insertAt, ".");

                return Double.Parse(strCoordinate);
            }
            catch
            {
                return 0.0D;
            }
        }
    }
}
