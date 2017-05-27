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
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using HFDIncidents.Domain;
using HFDIncidents.Web.ApiModels;

namespace HFDIncidents.Web.ApiControllers
{
    public class IncidentTypeController : ApiController
    {
        private IIncidentDataSource db;

        public IncidentTypeController(IIncidentDataSource db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<IncidentType>> GetIncidentTypes()
        {
            var incidentTypes = await db.IncidentTypes
                .OrderBy(it => it.Id)
                .ToListAsync();

            return incidentTypes
                .Select(it => new IncidentType(it));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }

            base.Dispose(disposing);
        }

    }
}
