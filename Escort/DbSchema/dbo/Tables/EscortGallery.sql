CREATE TABLE EscortGallery (
    ID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	EscortID INT NOT NULL Foreign Key References EscortDetail(EscortID),
    [FileName] VARCHAR(200),    
    MediaType TINYINT
);
