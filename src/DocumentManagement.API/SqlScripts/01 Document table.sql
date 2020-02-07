CREATE TABLE [dbo].[Document]
(
	[Id]            int IDENTITY(1,1) NOT NULL,
	[Name]			nvarchar(128)  NOT NULL,
	[Location]		nvarchar(256) NOT NULL,
	[Size]			bigint NOT NULL,
	[SortOrder]		int NOT NULL,

	CONSTRAINT [PK_Document] PRIMARY KEY CLUSTERED (Id ASC)
)