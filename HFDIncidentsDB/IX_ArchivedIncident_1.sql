CREATE NONCLUSTERED INDEX [IX_ArchivedIncident_1]
	ON [dbo].[ArchivedIncident] (
		[CallTimeOpened] ASC,
		[IncidentTypeId] ASC
	)
