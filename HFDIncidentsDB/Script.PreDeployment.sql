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

/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
USE [HFDIncidents]
GO
IF EXISTS(SELECT TOP 1 * FROM [HFDIncidents].[sys].[database_principals] WHERE name = 'HFDService') BEGIN
    DROP USER [HFDService]
END
IF EXISTS(SELECT TOP 1 * FROM [HFDIncidents].[sys].[database_principals] WHERE name = 'HFDWeb') BEGIN
    DROP USER [HFDWeb]
END
GO

USE [master]
GO
IF EXISTS(SELECT TOP 1 * FROM master.sys.server_principals WHERE name = 'HFDService') BEGIN
   DROP LOGIN [HFDService]
END
IF EXISTS(SELECT TOP 1 * FROM master.sys.server_principals WHERE name = 'HFDWeb') BEGIN
   DROP LOGIN [HFDWeb]
END
GO

USE [HFDIncidents]
GO
