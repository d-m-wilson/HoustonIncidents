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
using System.Configuration;
using Hangfire;
using HFDIncidents.IncidentLoader;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(HFDIncidents.Web.Startup))]

namespace HFDIncidents.Web
{
    public class Startup
    {
        internal static readonly string LoadIncidentsJobId;
        internal static readonly string LoadIncidentsJobCronExpression;
        internal static readonly string HangfireDashboardPath;

        static Startup()
        {
            LoadIncidentsJobId = ConfigurationManager.AppSettings[nameof(LoadIncidentsJobId)];
            LoadIncidentsJobCronExpression = ConfigurationManager.AppSettings[nameof(LoadIncidentsJobCronExpression)];
            HangfireDashboardPath = ConfigurationManager.AppSettings[nameof(HangfireDashboardPath)];
        }

        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration
                .UseSqlServerStorage("HFDIncidentsHangfire");

            var dashboardOptions = new DashboardOptions
            {
                AppPath = null,
                AuthorizationFilters = new[]
                {
                    new HangfireDashboardIPAddressAuthorizationFilter()
                },
            };

            app.UseHangfireDashboard(HangfireDashboardPath, dashboardOptions);

            app.UseHangfireServer();

            GlobalJobFilters.Filters.Add(new CustomSucceededJobExpirationTimeAttribute());

            RecurringJob.AddOrUpdate(
                LoadIncidentsJobId,
                () => ActiveIncidentLoader.LoadActiveIncidents(),
                LoadIncidentsJobCronExpression);
        }

    }
}
