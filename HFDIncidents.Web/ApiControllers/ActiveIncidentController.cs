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
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using HFDIncidents.Domain;
using HFDIncidents.Web.ApiModels;

namespace HFDIncidents.Web.ApiControllers
{
    public class ActiveIncidentController : ApiController
    {
        private IIncidentDataSource db;

        public ActiveIncidentController(IIncidentDataSource db)
        {
            this.db = db;
        }

        // GET api/ActiveIncident
        public async Task<IEnumerable<Incident>> GetActiveIncidents()
        {
            var activeIncidents = await db.ActiveIncidents
                .Include(ai => ai.IncidentType.Agency)
                .OrderBy(ai => ai.Id)
                .ToListAsync();

            return activeIncidents
                .Select(ai => new Incident(ai));
        }

        // GET api/ActiveIncident?incidentTypes=18,73
        [ResponseType(typeof(IEnumerable<Incident>))]
        public async Task<IHttpActionResult> GetActiveIncidents(string incidentTypes)
        {
            List<long> incidentTypeIds;

            try
            {
                incidentTypeIds = incidentTypes
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(item => long.Parse(item))
                    .ToList();
            }
            catch
            {
                return BadRequest();
            }

            var activeIncidents = await db.ActiveIncidents
                .Include(ai => ai.IncidentType.Agency)
                .Where(ai => incidentTypeIds.Contains(ai.IncidentTypeId.Value))
                .OrderBy(ai => ai.Id)
                .ToListAsync();

            var incidents = activeIncidents.Select(ai => new Incident(ai));

            return Ok(incidents);
        }

        // GET api/ActiveIncident/5
        [ResponseType(typeof(Incident))]
        public async Task<IHttpActionResult> GetActiveIncident(long id)
        {
            var activeincident = await db.ActiveIncidents
                .Include(ai => ai.IncidentType.Agency)
                .Where(ai => ai.Id == id)
                .SingleOrDefaultAsync();

            if (activeincident == null)
            {
                return NotFound();
            }

            Incident incident = new Incident(activeincident);

            return Ok(incident);
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