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
    public partial class HFDIncidentsContext
    {

        public void LogException(int nLevel, string strSource, string strMessage, string exception)
        {
            try
            {
                var logentry = new HFDServiceLog
                {
                    Level = nLevel,
                    Source = strSource.Clip(50),
                    Message = strMessage.Clip(200),
                    ExceptionText = exception,
                    LoggedDT = DateTime.Now
                };

                this.HFDServiceLogs.Add(logentry);
                this.SaveChanges();
            }
            catch
            {
            }
        }

        public void CreateServiceSession(out Int64 lNewSessionId, out Int64 lPrevSessionId)
        {
            var newSessionId = new System.Data.SqlClient.SqlParameter("newSessionId", System.Data.SqlDbType.BigInt)
            {
                Direction = System.Data.ParameterDirection.Output,
                IsNullable = false,
            };

            var prevSessionId = new System.Data.SqlClient.SqlParameter("prevSessionId", System.Data.SqlDbType.BigInt)
            {
                Direction = System.Data.ParameterDirection.Output,
                IsNullable = false,
            };

            Database.ExecuteSqlCommand(@"EXEC [dbo].[uspHFDServiceStartSession] @newSessionId OUTPUT, @prevSessionId OUTPUT", newSessionId, prevSessionId);

            lNewSessionId = (Int64)newSessionId.Value;
            lPrevSessionId = (Int64)prevSessionId.Value;
        }

    }
}
