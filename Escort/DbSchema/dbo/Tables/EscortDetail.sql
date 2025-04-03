CREATE TABLE EscortDetail (
    EscortID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL Foreign Key References UserDetail(UserId),
    Age INT,
    Bio VARCHAR(1000),
    SexualPreferences VARCHAR(50),
    Height TINYINT,
    BodyType VARCHAR(50),
    BankAccountHolderName VARCHAR(110),
    BankAccountNumber VARCHAR(50),
    BSBNumber VARCHAR(10),    
    Eyes VARCHAR(20),
    Category VARCHAR(50),
    [Language] VARCHAR(50)
);
