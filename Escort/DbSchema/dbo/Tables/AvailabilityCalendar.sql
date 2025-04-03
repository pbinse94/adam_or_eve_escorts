CREATE TABLE AvailabilityCalendar (
    ID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    EscortID INT NOT NULL Foreign Key References EscortDetail(EscortID),
    FromTime VARCHAR(10) NULL,
    ToTime VARCHAR(10) NULL,
    DayNumber TINYINT NULL,
    IsNotAvailable BIT,
    IsAvailable24X7 BIT
);
