/*
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
*/
CREATE TABLE [dbo].[ArchivedIncident]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [RetrievedDT] DATETIME NOT NULL, 
    [Address] VARCHAR(255) NULL, 
    [CrossStreet] VARCHAR(255) NULL, 
    [KeyMap] VARCHAR(10) NULL, 
    [Latitude] FLOAT NULL, 
    [Longitude] FLOAT NULL, 
    [CombinedResponse] BIT NOT NULL DEFAULT (0), 
    [CallTimeOpened] DATETIME NULL, 
    [IncidentTypeId] BIGINT NULL, 
    [AlarmLevel] INT NOT NULL DEFAULT (0), 
    [NumberOfUnits] INT NOT NULL DEFAULT (0), 
    [Units] VARCHAR(255) NULL, 
    [LastSeenDT] DATETIME NOT NULL, 
	[ArchivedDT] DATETIME NOT NULL DEFAULT (GETDATE()), 
	[Status] VARCHAR(20) NULL,
	[Notes] VARCHAR(100) NULL, 
    CONSTRAINT [FK_ArchivedIncident_IncidentType] FOREIGN KEY ([IncidentTypeId]) REFERENCES [IncidentType]([Id])
)
