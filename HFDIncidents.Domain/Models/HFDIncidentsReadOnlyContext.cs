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
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace HFDIncidents.Domain.Models
{
    public sealed class HFDIncidentsReadOnlyContext : DbContext, IIncidentDataSource
    {
        public HFDIncidentsReadOnlyContext()
            : base("Name=HFDIncidentsReadOnlyContext")
        {
            Database.SetInitializer<Models.HFDIncidentsReadOnlyContext>(null);
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        IQueryable<ActiveIncident> IIncidentDataSource.ActiveIncidents
        {
            get
            {
                return Set<ActiveIncident>()
                    .AsNoTracking()
                    .Include(ai => ai.IncidentType.Agency);
            }
        }

        IQueryable<ArchivedIncident> IIncidentDataSource.ArchivedIncidents
        {
            get
            {
                return Set<ArchivedIncident>()
                    .AsNoTracking()
                    .Include(ai => ai.IncidentType.Agency);
            }
        }

        IQueryable<IncidentType> IIncidentDataSource.IncidentTypes
        {
            get 
            {
                return Set<IncidentType>()
                    .AsNoTracking()
                    .Include(it => it.Agency);
            }
        }

        public override int SaveChanges()
        {
            throw new InvalidOperationException("This context is read-only.");
        }

        public override System.Threading.Tasks.Task<int> SaveChangesAsync()
        {
            throw new InvalidOperationException("This context is read-only.");
        }

        public override System.Threading.Tasks.Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("This context is read-only.");
        }

        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArchivedIncident>();
            modelBuilder.Entity<IncidentType>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
