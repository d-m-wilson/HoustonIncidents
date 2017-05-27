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
CREATE PROCEDURE [dbo].[uspHFDServiceStartSession]
	@NewSessionId BIGINT OUTPUT,
	@PrevSessionId BIGINT OUTPUT
AS
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
	BEGIN TRANSACTION
	SET XACT_ABORT ON

	IF EXISTS(SELECT TOP 1 * FROM [dbo].[HFDServiceSession])
		SELECT @PrevSessionId = MAX([Id]) FROM [dbo].[HFDServiceSession]
	ELSE
		SELECT @PrevSessionId = -1

	INSERT INTO [dbo].[HFDServiceSession] DEFAULT VALUES
	SELECT @NewSessionId = SCOPE_IDENTITY()

	IF @PrevSessionId = -1
		SELECT @PrevSessionId = @NewSessionId - 1

	DELETE FROM [dbo].[HFDServiceSession] WHERE [Id] < @NewSessionId

	COMMIT TRANSACTION
RETURN 0
