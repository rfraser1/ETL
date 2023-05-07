CREATE TABLE [dbo].[Rainfall]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [session] VARCHAR(256) NOT NULL, 
    [xref] DECIMAL(18, 3) NOT NULL, 
    [yref] DECIMAL(18, 3) NOT NULL, 
    [value] DECIMAL(18, 3) NOT NULL, 
    [date] DATE NOT NULL
)
