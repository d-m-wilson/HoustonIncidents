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
using System.Collections.Generic;
using System.Web.Mvc;

namespace HFDIncidents.Web
{
    public static class DefaultValues
    {
        public const int MinPageSize = 25;
        public const int DefaultPageSize = 100;
        public const string DefaultPageSizeString = "100";
        public const int MaxPageSize = 300;
        public const string AllRecords = "ALL";

        public const double Latitude = 29.7605;
        public const double Longitude = -95.3666;

        public static readonly IReadOnlyCollection<string> PageSizes;

        public static long DefaultIncidentTypeId;

        static DefaultValues()
        {
            PageSizes = new List<string>(new [] { "25", "50", "100", "300", AllRecords }).AsReadOnly();

            Int64.TryParse(AppSettings.DefaultIncidentTypeNumber, out DefaultIncidentTypeId);
        }

        public static SelectList ItemsPerPageList
        {
            get
            {
                return new SelectList(PageSizes, DefaultPageSizeString);
            }
        }
    }
}