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
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
USE [master]
GO
CREATE LOGIN [HFDService] WITH PASSWORD=N'hfdSvcSqlp455!', DEFAULT_DATABASE=[master], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
USE [HFDIncidents]
GO
IF EXISTS(SELECT TOP 1 * FROM [HFDIncidents].[sys].[database_principals] WHERE name = 'HFDService') BEGIN
    DROP USER [HFDService]
END
CREATE USER [HFDService] FOR LOGIN [HFDService] WITH DEFAULT_SCHEMA=[dbo]
GO
USE [HFDIncidents]
GO
EXEC sp_addrolemember N'db_datareader', N'HFDService'
GO
EXEC sp_addrolemember N'db_datawriter', N'HFDService'
GO
EXEC sp_addrolemember N'db_owner', N'HFDService'
GO

USE [master]
GO
CREATE LOGIN [HFDWeb] WITH PASSWORD=N'aSecurePassword1#', DEFAULT_DATABASE=[master], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
USE [HFDIncidents]
GO
IF EXISTS(SELECT TOP 1 * FROM [HFDIncidents].[sys].[database_principals] WHERE name = 'HFDWeb') BEGIN
    DROP USER [HFDWeb]
END
CREATE USER [HFDWeb] FOR LOGIN [HFDWeb] WITH DEFAULT_SCHEMA=[dbo]
GO
USE [HFDIncidents]
GO
EXEC sp_addrolemember N'db_datareader', N'HFDWeb'
GO

SET IDENTITY_INSERT [HFDIncidents].[dbo].[Agency] ON
IF NOT EXISTS(SELECT TOP 1 [Id] FROM [HFDIncidents].[dbo].[Agency] WHERE [Code] IS NULL)
	INSERT INTO [HFDIncidents].[dbo].[Agency] ( [Id], [Name], [Code], [ShortName] ) VALUES  ( 1, 'Unknown', NULL, 'Unknown' )
IF NOT EXISTS(SELECT TOP 1 [Id] FROM [HFDIncidents].[dbo].[Agency] WHERE [Code] = 'F')
	INSERT INTO [HFDIncidents].[dbo].[Agency] ( [Id], [Name], [Code], [ShortName] ) VALUES  ( 2, 'Houston Fire Department', 'F', 'Fire' )
IF NOT EXISTS(SELECT TOP 1 [Id] FROM [HFDIncidents].[dbo].[Agency] WHERE [Code] = 'P')
	INSERT INTO [HFDIncidents].[dbo].[Agency] ( [Id], [Name], [Code], [ShortName] ) VALUES  ( 3, 'Houston Police Department', 'P', 'Police' )
SET IDENTITY_INSERT [HFDIncidents].[dbo].[Agency] OFF

SET IDENTITY_INSERT [HFDIncidents].[dbo].[IncidentType] ON
IF NOT EXISTS(SELECT TOP 1 [Id] FROM [HFDIncidents].[dbo].[IncidentType] WHERE [AgencyId] = 2 AND [Name] = 'EMS Event')
	INSERT INTO [HFDIncidents].[dbo].[IncidentType] ( [Id], [AgencyId], [Name] ) VALUES  ( 1, 2, 'EMS Event' )
SET IDENTITY_INSERT [HFDIncidents].[dbo].[IncidentType] OFF
