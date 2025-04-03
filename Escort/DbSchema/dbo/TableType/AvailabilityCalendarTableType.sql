CREATE TYPE [dbo].[AvailabilityCalendarTableType] AS TABLE(
	ID INT,
	EscortID INT NOT NULL,
	FromTime VARCHAR(10) NULL,
	ToTime VARCHAR(10) NULL,
	DayNumber TINYINT NULL,
	IsNotAvailable BIT,
	IsAvailable24X7 BIT
)
GO