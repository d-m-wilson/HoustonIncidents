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
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using HFDIncidents.Domain;
using HFDIncidents.Domain.Models;
using HFDIncidents.Web.ViewModels;
using X.PagedList;

namespace HFDIncidents.Web.Controllers
{
    public class HomeController : Controller
    {
        private IIncidentDataSource db;

        public HomeController(IIncidentDataSource db)
        {
            this.db = db;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        // GET: Home/Search
        public async Task<ActionResult> Search(string itemsPerPage, int? page, string fromDate, string toDate, List<long> types)
        {
            if (!page.HasValue || page.Value < 1)
            {
                page = 1;
            }

            bool showAllRecords = string.Equals(itemsPerPage, DefaultValues.AllRecords);

            int itemsPerPageValue;

            if (!showAllRecords)
            {
                int.TryParse(itemsPerPage, out itemsPerPageValue);

                if (itemsPerPageValue < 1)
                {
                    itemsPerPageValue = DefaultValues.DefaultPageSize;
                }
                else if (itemsPerPageValue > DefaultValues.MaxPageSize)
                {
                    itemsPerPageValue = DefaultValues.MaxPageSize;
                }
            }
            else
            {
                itemsPerPageValue = int.MaxValue;
            }

            DateTime from;
            DateTime to;

            if (string.IsNullOrWhiteSpace(fromDate) || !DateTime.TryParse(fromDate, out from))
            {
                from = DateTime.Today.AddDays(-30);
            }
            else
            {
                from = from.Date;
            }

            if (string.IsNullOrWhiteSpace(toDate) || !DateTime.TryParse(toDate, out to))
            {
                to = DateTime.Today.AddMinutes(1439);
            }
            else
            {
                to = to.Date.AddMinutes(1439);
            }

            var incidentTypes = await db.IncidentTypes
                .OrderBy(it => it.Name)
                .ToListAsync();

            IEnumerable<SelectListItem> incidentTypesListItems;

            if (types == null)
            {
                incidentTypesListItems = incidentTypes.Select(it => new SelectListItem { Text = it.Name, Value = it.Id.ToString(), Selected = it.Id == DefaultValues.DefaultIncidentTypeId });
                types = incidentTypes.Where(it => it.Id == DefaultValues.DefaultIncidentTypeId).Select(it => it.Id).ToList();
            }
            else
            {
                incidentTypesListItems = incidentTypes.Select(it => new SelectListItem { Text = it.Name, Value = it.Id.ToString(), Selected = types.Contains(it.Id) });
            }

            var incidentsQuery = db.ArchivedIncidents
                .Where(ai => ai.CallTimeOpened >= from && ai.CallTimeOpened <= to && types.Contains(ai.IncidentTypeId.Value));

            var pagedList = await incidentsQuery.OrderBy(i => i.CallTimeOpened).ToPagedListAsync(page.Value, itemsPerPageValue);

            var vm = new SearchViewModel
            {
                IncidentTypes = incidentTypesListItems,
                FromDate = from,
                ToDate = to,
                types = types,
                Page = page.Value,
                PageSize = showAllRecords ? DefaultValues.AllRecords : itemsPerPageValue.ToString(),
                Incidents = pagedList,
            };

            return View(vm);
        }

        // GET: Home/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ArchivedIncident archivedIncident = await db.ArchivedIncidents
                .Where(ai => ai.Id == id)
                .FirstOrDefaultAsync();

            if (archivedIncident == null)
            {
                return HttpNotFound();
            }

            return View(archivedIncident);
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