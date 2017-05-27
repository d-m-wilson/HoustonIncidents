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
using System.Data.Entity;
using System.Linq;
using System.Threading;
using Hangfire;
using HFDIncidents.Domain;
using HFDIncidents.Domain.Models;
using Newtonsoft.Json;

namespace HFDIncidents.IncidentLoader
{
    public static class ActiveIncidentLoader
    {
        public const int LOAD_ACTIVE_INCIDENTS_MAX_RETRIES = 3;

        private static bool EnableIncidentLoaderDebugLogging;

        [AutomaticRetry(Attempts = LOAD_ACTIVE_INCIDENTS_MAX_RETRIES, LogEvents = true, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public static void LoadActiveIncidents()
        {
            int incidentsAdded = 0;
            int incidentsUpdated = 0;
            int incidentsArchived = 0;
            int totalActiveIncidentsBefore;
            int existingIncidentsCount;
            int totalActiveIncidentsAfter;
            int incidentRecordsRetrieved;

            if (!bool.TryParse(
                ConfigurationManager.AppSettings[nameof(EnableIncidentLoaderDebugLogging)], out EnableIncidentLoaderDebugLogging))
            {
                EnableIncidentLoaderDebugLogging = false;
            }

            var tranStarIncidents = new List<ActiveIncidentRecord>();

            // Try to load and process incidents from Houston TranStar
            try
            {
                using (var client = new System.Net.WebClient())
                {
                    var url = "http://traffic.houstontranstar.org/data/layers/incidents_json.js";
                    var json = client.DownloadString(url);
                    var data = JsonConvert.DeserializeObject<HoustonTranStarIncidentWrapper>(json);
                    var incidents = data.incidents;

                    if (incidents != null)
                    {
                        foreach (var i in incidents)
                        {
                            DateTime datePart;
                            if (i.date == "yesterday")
                            {
                                datePart = DateTime.Today.AddDays(-1);
                            }
                            else if (i.date == "today" || !DateTime.TryParse(i.date, out datePart))
                            {
                                datePart = DateTime.Today;
                            }

                            DateTime timePart;
                            DateTime.TryParse(i.time, out timePart);

                            var combinedDateTime = new DateTime(
                                datePart.Year,
                                datePart.Month,
                                datePart.Day,
                                timePart.Hour,
                                timePart.Minute,
                                timePart.Second,
                                DateTimeKind.Local);

                            var ai = new ActiveIncidentRecord
                            {
                                Agency = "T",
                                Address = i.location,
                                AlarmLevel = "0",
                                CallTimeOpened = combinedDateTime.ToString(),
                                CombinedResponse = "N",
                                CrossStreet = null,
                                IncidentType = i.desc,
                                KeyMap = null,
                                NumberOfUnits = i.veh,
                                Units = null,
                                XCoord = i.lng?.Replace(".", ""),
                                YCoord = i.lat?.Replace(".", ""),
                                Status = i.status?.Clip(20) ?? "Unknown",
                                Notes = i.lanes?.Clip(100),
                            };

                            tranStarIncidents.Add(ai);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (HFDIncidentsContext context = new HFDIncidentsContext())
                {
                    context.LogException(9, "HoustonTranStar", ex.Message, ex.ToString());
                }
            }

            // There is a recurring job in Hangfire to run this method every minute that is evenly divisible by 5. (I.e. 1:00, 1:05, 1:10, etc.)
            // Based on my observations, it is likely that the HFD/HPD active incident list is being updated in the same interval.
            // Also, based on my observations, if you query their web service during its update cycle, there is a (sort of) random chance
            // of getting back an EMPTY list of incidents, which causes problems for this application. I think that simply waiting for
            // at least 30 seconds will greatly reduce the risk of querying the HFD/HPD web service while it is updating. - DMW 20160629
            Thread.Sleep(TimeSpan.FromSeconds(30));

            using (HFDIncidentsContext context = new HFDIncidentsContext())
            {
                ActiveIncidentResult wsResult;
                DateTime dtRetrieved;
                long lCurrentSessionId;
                long lPreviousSessionId;

                using (var client = new HoustonFireDept.WebService1SoapClient())
                {
                    try
                    {
                        var json = client.GetIncidents();
                        dtRetrieved = DateTime.Now;
                        wsResult = JsonConvert.DeserializeObject<ActiveIncidentResult>(json);
                    }
                    catch (Exception ex)
                    {
                        context.LogException(10, nameof(ActiveIncidentLoader), ex.Message, ex.ToString());
                        throw new Exception(ex.Message);
                    }
                }

                if (wsResult == null || wsResult.ActiveIncidentDataTable == null
                    || wsResult.ActiveIncidentDataTable.Length == 0)
                {
                    // Despite our best efforts to avoid it, we managed to get an empty list of incidents.

                    var errorMessage = string.Format(
                        "{0}.{1}() succeeded, but returned no incidents.",
                        nameof(HoustonFireDept.WebService1SoapClient),
                        nameof(HoustonFireDept.WebService1SoapClient.GetIncidents));

                    context.LogException(
                        9,
                        nameof(ActiveIncidentLoader),
                        errorMessage,
                        null);

                    // Throw an exception, and Hangfire will re-queue the job to try again if it is configured to do so.
                    throw new Exception(errorMessage);
                }

                context.CreateServiceSession(out lCurrentSessionId, out lPreviousSessionId);

                var agencies = context.Agencies.ToList();

                var incidentTypes = context.IncidentTypes.Include(it => it.Agency).ToList();

                var existingIncidents = context.ActiveIncidents
                    .Include(i => i.IncidentType.Agency)
                    .Where(i => i.ServiceSessionId >= lPreviousSessionId)
                    .ToList();

                existingIncidentsCount = existingIncidents.Count;
                totalActiveIncidentsBefore = context.ActiveIncidents.Count();

                // FOR DEBUGGING ONLY!
                var sentinel = existingIncidents.FirstOrDefault(i => i.ServiceSessionId > lPreviousSessionId);
                if (sentinel != null)
                {
                    context.LogException(10, nameof(ActiveIncidentLoader), "sentinel is non-null", null);
                }

                incidentRecordsRetrieved = wsResult.ActiveIncidentDataTable.Length;

                var activeIncidentRecords = new List<ActiveIncidentRecord>(wsResult.ActiveIncidentDataTable);

                if (tranStarIncidents.Count > 0)
                {
                    activeIncidentRecords.AddRange(tranStarIncidents);
                }

                foreach (var i in activeIncidentRecords)
                {
                    IncidentType incidentType = null;
                    ActiveIncident ai = null;

                    Agency agency = agencies.FirstOrDefault(a => a.Code == i.Agency);

                    if (agency == null)
                    {
                        agency = agencies.First(a => a.Code == null);
                    }

                    incidentType = incidentTypes
                        .Where(it => it.Agency == agency && String.Compare(it.Name, i.IncidentType, true) == 0)
                        .FirstOrDefault();

                    if (incidentType == null)
                    {
                        // auto-create incident type
                        incidentType = new IncidentType
                        {
                            Agency = agency,
                            Name = i.IncidentType.Trim(),
                        };

                        incidentTypes.Add(incidentType);
                    }
                    else
                    {
                        // Check for existing, duplicate record here!
                        ai = existingIncidents
                            //.Include(a => a.IncidentType)
                            //.Include(a => a.Agency)
                            .FirstOrDefault(a =>
                                a.IncidentTypeId == incidentType.Id &&
                                a.Address == i.Address?.Trim() &&
                                a.KeyMap == i.KeyMap?.Trim()
                            );
                    }

                    if (ai != null)
                    {
                        var blCombinedResponse = String.Compare(i.CombinedResponse, "Y", true) == 0;

                        if (!ai.CombinedResponse && blCombinedResponse)
                        {
                            ai.CombinedResponse = blCombinedResponse;
                        }

                        if (ai.AlarmLevel < i.AlarmLevelInt)
                        {
                            ai.AlarmLevel = i.AlarmLevelInt;
                        }

                        ai.CurrentUnits = i.Units?.Trim()?.Trim(';');

                        ai.Units = MergeUnits(ai.Units, i.Units, context);

                        if (!string.IsNullOrWhiteSpace(ai.Units))
                        {
                            ai.Units = ai.Units.Trim(';');
                        }

                        if (!string.IsNullOrWhiteSpace(ai.CurrentUnits))
                        {
                            try { ai.NumberOfUnits = ai.CurrentUnits.Split(';').Length; }
                            catch { ai.NumberOfUnits = 0; }
                        }
                        else if (!string.IsNullOrWhiteSpace(ai.Units))
                        {
                            try { ai.NumberOfUnits = ai.Units.Split(';').Length; }
                            catch { ai.NumberOfUnits = 0; }
                        }
                        else
                        {
                            ai.NumberOfUnits = 0;
                        }

                        ai.Status = i.Status?.Trim();
                        ai.Notes = string.IsNullOrWhiteSpace(i.Notes) ? null : i.Notes?.Trim();

                        incidentsUpdated++;
                    }
                    else
                    {
                        ai = new ActiveIncident
                        {
                            RetrievedDT = dtRetrieved,
                            Address = i.Address?.Trim(),
                            CrossStreet = i.CrossStreet?.Trim(),
                            KeyMap = i.KeyMap?.Trim(),
                            Latitude = i.Latitude,
                            Longitude = i.Longitude,
                            CombinedResponse = string.Equals(i.CombinedResponse?.Trim(), "Y", StringComparison.InvariantCultureIgnoreCase),
                            CallTimeOpened = i.CallTimeOpenedDT,
                            IncidentType = incidentType,
                            AlarmLevel = i.AlarmLevelInt,
                            NumberOfUnits = i.NumberOfUnitsInt,
                            CurrentUnits = i.Units?.Trim()?.Trim(';'),
                            Units = i.Units?.Trim()?.Trim(';'),
                            Status = i.Status?.Trim(),
                            Notes = i.Notes?.Trim(),
                        };

                        if (ai.Latitude.HasValue && ai.Latitude == 0)
                        {
                            ai.Latitude = null;
                        }

                        if (ai.Longitude.HasValue && ai.Longitude == 0)
                        {
                            ai.Longitude = null;
                        }

                        // Latitude < 0 and Longitude > 0 can't be right for North America.
                        // We'll assume the coordinates are reversed, and swap them.
                        if (ai.Latitude.HasValue && ai.Latitude.Value < 0
                            && ai.Longitude.HasValue && ai.Longitude > 0)
                        {
                            var swap = ai.Longitude.Value;
                            ai.Longitude = ai.Latitude.Value;
                            ai.Latitude = swap;
                        }

                        context.ActiveIncidents.Add(ai);
                        incidentsAdded++;
                    }

                    ai.LastSeenDT = dtRetrieved;
                    ai.ServiceSessionId = lCurrentSessionId;
                }

                // Get all ActiveIncidents that were not added/updated this session
                var staleIncidents = context.ActiveIncidents
                    .Include(i => i.IncidentType.Agency)
                    .Where(i => i.ServiceSessionId < lPreviousSessionId)
                    .ToList();

                foreach (var incident in staleIncidents)
                {
                    var archivedIncident = ArchivedIncident.CreateFromActiveIncident(incident);
                    archivedIncident.ArchivedDT = dtRetrieved;
                    context.ArchivedIncidents.Add(archivedIncident);
                    context.ActiveIncidents.Remove(incident);
                    incidentsArchived++;
                }

                try
                {
                    context.SaveChanges();

                    if (EnableIncidentLoaderDebugLogging)
                    {
                        totalActiveIncidentsAfter = context.ActiveIncidents.Count();

                        var message = string.Format(
                            "totalBefore: {0} existingBefore: {1} recordsRetrieved: {2} added: {3} updated: {4} archived: {5} totalAfter: {6}",
                            totalActiveIncidentsBefore,
                            existingIncidentsCount,
                            incidentRecordsRetrieved,
                            incidentsAdded,
                            incidentsUpdated,
                            incidentsArchived,
                            totalActiveIncidentsAfter);

                        context.LogException(0, nameof(ActiveIncidentLoader), message, null);
                    }
                }
                catch (Exception ex)
                {
                    context.LogException(
                        10,
                        nameof(ActiveIncidentLoader),
                        string.Format("Unable to save changes. {0}", ex.Message),
                        ex.ToString());
                }
            }
        }

        private static string MergeUnits(string existingUnitsString, string updatedUnitsString, HFDIncidentsContext context)
        {
            existingUnitsString = existingUnitsString ?? "";

            if (string.IsNullOrWhiteSpace(updatedUnitsString))
            {
                return existingUnitsString;
            }

            if (string.Equals(existingUnitsString, updatedUnitsString, StringComparison.InvariantCulture))
            {
                return existingUnitsString;
            }

            try
            {
                var existingUnitsList = existingUnitsString
                    .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                var updatedUnitsList = updatedUnitsString
                    .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                var mergedUnitsList = new List<string>(existingUnitsList);
                mergedUnitsList.AddRange(updatedUnitsList);

                for (int index = 0; index < mergedUnitsList.Count; index++)
                {
                    var currentUnit = mergedUnitsList[index];
                    var highestIndex = mergedUnitsList.LastIndexOf(currentUnit);

                    while (highestIndex != index)
                    {
                        mergedUnitsList.RemoveAt(highestIndex);
                        highestIndex = mergedUnitsList.LastIndexOf(currentUnit);
                    }
                }

                var mergedUnitsString = string.Join(";", mergedUnitsList.ToArray());

                if (EnableIncidentLoaderDebugLogging)
                {
                    if (mergedUnitsList.Count > existingUnitsList.Count)
                    {
                        var message = string.Format(
                            "{0} units added.\r\nExisting: {1}\r\nUpdate:   {2}\r\nMerged:   {3}",
                            mergedUnitsList.Count - existingUnitsList.Count,
                            existingUnitsString,
                            updatedUnitsString,
                            mergedUnitsString);

                        context.LogException(0, nameof(ActiveIncidentLoader), message, null);
                    }
                }

                return string.Join(";", mergedUnitsList);
            }
            catch (Exception ex)
            {
                context.LogException(
                    9,
                    nameof(ActiveIncidentLoader),
                    string.Format("Error in MergeUnits(): {0}", ex.Message),
                    ex.ToString());

                return existingUnitsString.Length > updatedUnitsString.Length
                    ? existingUnitsString : updatedUnitsString;
            }
        }

    }
}
