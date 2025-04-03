CREATE TABLE EscortLocation (
	Id int NOT NULL PRIMARY KEY IDENTITY(1,1),
	EscortID INT NOT NULL Foreign Key References EscortDetail(EscortID),
    AddressType TINYINT,
    [State] INT,
    City INT
);
