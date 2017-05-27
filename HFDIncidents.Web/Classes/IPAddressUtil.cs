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
using System.Net;

namespace HFDIncidents.Web
{
    public static class IPAddressUtil
    {
        public static bool IsInSubnet(string ipv4Address, string ipv4SubnetCIDR)
        {
            if (string.IsNullOrWhiteSpace(ipv4Address))
            {
                throw new ArgumentOutOfRangeException(nameof(ipv4Address));
            }

            if (string.IsNullOrWhiteSpace(ipv4SubnetCIDR)
                || !ipv4SubnetCIDR.Contains("/"))
            {
                throw new ArgumentOutOfRangeException(nameof(ipv4SubnetCIDR));
            }

            string[] subnetParts;
            IPAddress subnetIPAddress, ipAddress;
            int subnetIPAddressAsInt32;
            int subnetMaskBits;

            try
            {
                subnetParts = ipv4SubnetCIDR.Split('/');
                subnetIPAddress = IPAddress.Parse(subnetParts[0]);
                subnetIPAddressAsInt32 = BitConverter.ToInt32(subnetIPAddress.GetAddressBytes(), 0);

                subnetMaskBits = int.Parse(subnetParts[1]);
            }
            catch (Exception ex1)
            {
                throw new ArgumentOutOfRangeException(nameof(ipv4SubnetCIDR), ex1);
            }

            try
            {
                ipAddress = IPAddress.Parse(ipv4Address);
            }
            catch (Exception ex2)
            {
                throw new ArgumentOutOfRangeException(nameof(ipv4Address), ex2);
            }

            if (subnetIPAddress.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
            {
                throw new ArgumentOutOfRangeException(nameof(ipv4SubnetCIDR), "Only IPv4 is supported.");
            }

            if (ipAddress.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
            {
                throw new ArgumentOutOfRangeException(nameof(ipv4Address), "Only IPv4 is supported.");
            }

            var ipAddressAsInt32 = BitConverter.ToInt32(ipAddress.GetAddressBytes(), 0);

            var subnetMask = IPAddress.HostToNetworkOrder(-1 << (32 - subnetMaskBits));

            var result = ((subnetIPAddressAsInt32 & subnetMask) == (ipAddressAsInt32 & subnetMask));

            return result;
        }
    }
}