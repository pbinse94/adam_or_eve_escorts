CREATE TYPE [dbo].[LocationTableType] AS TABLE(
	EscortID INT NOT NULL,
	AddressType TINYINT,
	[State] INT,
	City INT
)
GO