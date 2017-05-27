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
using System.Data.Entity;
using System.Linq;
using HFDIncidents.Domain.Models.Mapping;

namespace HFDIncidents.Domain.Models
{
    public partial class HFDIncidentsContext : DbContext, IIncidentDataSource
    {
        static HFDIncidentsContext()
        {
            Database.SetInitializer<HFDIncidentsContext>(null);
        }

        public HFDIncidentsContext()
            : base("Name=HFDIncidentsContext")
        {
        }

        public DbSet<ActiveIncident> ActiveIncidents { get; set; }
        public DbSet<Agency> Agencies { get; set; }
        public DbSet<ArchivedIncident> ArchivedIncidents { get; set; }
        public DbSet<HFDServiceLog> HFDServiceLogs { get; set; }
        public DbSet<IncidentType> IncidentTypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ActiveIncidentMap());
            modelBuilder.Configurations.Add(new AgencyMap());
            modelBuilder.Configurations.Add(new ArchivedIncidentMap());
            modelBuilder.Configurations.Add(new HFDServiceLogMap());
            modelBuilder.Configurations.Add(new IncidentTypeMap());
        }

        IQueryable<ActiveIncident> IIncidentDataSource.ActiveIncidents
        {
            get { return ActiveIncidents.Include(ai => ai.IncidentType.Agency); }
        }

        IQueryable<ArchivedIncident> IIncidentDataSource.ArchivedIncidents
        {
            get { return ArchivedIncidents.Include(ai => ai.IncidentType.Agency); }
        }

        IQueryable<IncidentType> IIncidentDataSource.IncidentTypes
        {
            get { return IncidentTypes.Include(it => it.Agency); }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
