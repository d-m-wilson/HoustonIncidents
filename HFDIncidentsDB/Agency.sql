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
CREATE TABLE [dbo].[Agency]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] VARCHAR(255) NOT NULL, 
    [Code] VARCHAR(10) NULL, 
    [ShortName] VARCHAR(10) NOT NULL, 
    CONSTRAINT [UQ_Agency_Code] UNIQUE ([Code]),
	CONSTRAINT [UQ_Agency_Name] UNIQUE ([Name]),
	CONSTRAINT [UQ_Agency_ShortName] UNIQUE ([ShortName])
)
