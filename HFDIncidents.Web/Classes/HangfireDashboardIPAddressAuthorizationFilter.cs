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
using System.Configuration;
using Hangfire.Dashboard;

namespace HFDIncidents.Web
{
    public class HangfireDashboardIPAddressAuthorizationFilter : IAuthorizationFilter
    {
        private static readonly IReadOnlyCollection<string> authorizedAddresses;

        static HangfireDashboardIPAddressAuthorizationFilter()
        {
            string HangfireDashboardAuthorizedAddressesCSV = ConfigurationManager.AppSettings[nameof(HangfireDashboardAuthorizedAddressesCSV)];

            var items = HangfireDashboardAuthorizedAddressesCSV.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            authorizedAddresses = new List<string>(items).AsReadOnly();
        }

        public bool Authorize(IDictionary<string, object> owinEnvironment)
        {
            if (authorizedAddresses == null)
            {
                return false;
            }

            var myObject = owinEnvironment["server.RemoteIpAddress"];
            var remoteIpAddress = owinEnvironment["server.RemoteIpAddress"] as string;

            if (string.IsNullOrWhiteSpace(remoteIpAddress))
            {
                return false;
            }

            foreach (var authorizedAddress in authorizedAddresses)
            {
                if (string.Equals(remoteIpAddress, authorizedAddress, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }

                if (!remoteIpAddress.Contains(":") && authorizedAddress.Contains("/"))
                {
                    var result = IPAddressUtil.IsInSubnet(remoteIpAddress, authorizedAddress);

                    if (result)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}