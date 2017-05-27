CREATE NONCLUSTERED INDEX [IX_ArchivedIncident_2]
	ON [dbo].[ArchivedIncident] (
		[IncidentTypeId] ASC,
		[CallTimeOpened] ASC
	)
