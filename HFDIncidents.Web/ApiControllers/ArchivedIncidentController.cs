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
    public class ArchivedIncidentController : ApiController
    {
        private const int MAX_RESULTS = 500;
        private const string INVALID_ARGUMENT_MESSAGE_FORMAT = "Invalid argument supplied for parameter: {0}";
        private IIncidentDataSource db;

        public ArchivedIncidentController(IIncidentDataSource db)
        {
            this.db = db;
        }

        // GET api/ArchivedIncident
        public async Task<IEnumerable<Incident>> GetArchivedIncidents()
        {
            var archivedIncidents = await db.ArchivedIncidents
                .Include(ai => ai.IncidentType.Agency)
                .OrderBy(ai => ai.Id)
                .Take(MAX_RESULTS)
                .ToListAsync();

            return archivedIncidents
                .Select(ai => new Incident(ai));
        }

        // GET api/ArchivedIncident?from=2015-05-25T23:00:00-0500&to=2015-05-26T02:29:59-0500&incidentTypes=27,32&skip=0&count=20
        [ResponseType(typeof(IEnumerable<Incident>))]
        public async Task<IHttpActionResult> GetArchivedIncidents(string from, string to, string incidentTypes, int skip, int count)
        {
            if (skip < 0)
            {
                skip = 0;
            }

            if (count > MAX_RESULTS)
            {
                count = MAX_RESULTS;
            }

            DateTime fromDT, toDT;

            try
            {
                fromDT = DateTime.Parse(from, null, System.Globalization.DateTimeStyles.RoundtripKind);
            }
            catch
            {
                return BadRequest(string.Format(INVALID_ARGUMENT_MESSAGE_FORMAT, nameof(from)));
            }

            try
            {
                toDT = DateTime.Parse(to, null, System.Globalization.DateTimeStyles.RoundtripKind);
            }
            catch
            {
                return BadRequest(string.Format(INVALID_ARGUMENT_MESSAGE_FORMAT, nameof(to)));
            }

            var query = db.ArchivedIncidents
                .Include(ai => ai.IncidentType.Agency)
                .Where(ai => ai.CallTimeOpened >= fromDT && ai.CallTimeOpened <= toDT);

            List<long> incidentTypeIds;

            if (!string.IsNullOrWhiteSpace(incidentTypes))
            {
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

                query = query
                    .Where(ai => incidentTypeIds.Contains(ai.IncidentTypeId.Value));
            }

            query = query
                .OrderBy(ai => ai.Id)
                .Skip(skip)
                .Take(count);

            var archivedIncidents = await query
                .ToListAsync();

            return Ok(archivedIncidents
                .Select(ai => new Incident(ai)));
        }

        // GET api/ActiveIncident/5
        [ResponseType(typeof(Incident))]
        public async Task<IHttpActionResult> GetArchivedIncident(long id)
        {
            var archivedincident = await db.ArchivedIncidents
                .Include(ai => ai.IncidentType.Agency)
                .Where(ai => ai.Id == id)
                .SingleOrDefaultAsync();

            if (archivedincident == null)
            {
                return NotFound();
            }

            var incident = new Incident(archivedincident);

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