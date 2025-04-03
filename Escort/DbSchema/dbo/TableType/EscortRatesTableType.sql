CREATE TYPE [dbo].[EscortRatesTableType] AS TABLE(
	Id INT,
	EscortID INT NOT NULL,
	Duration int,
	InCallRate decimal(10,2),
	OutCallRate	decimal(10,2)
)
GO