CREATE TABLE EscortRates (
	Id int NOT NULL PRIMARY KEY IDENTITY(1,1),
	EscortID INT NOT NULL Foreign Key References EscortDetail(EscortID),
	Duration int,
	InCallRate decimal(10,2),
	OutCallRate	decimal(10,2)
);
