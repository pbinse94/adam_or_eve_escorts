CREATE PROCEDURE GetEscortDetailsById 
	@UserId INT
AS
-- exec GetEscortDetailsById 11
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @EscortID INT  

	SELECT @EscortID = EscortID FROM EscortDetail WHERE UserId = @UserId
	
    SELECT EscortID, UserId, Age, Bio, Height, BodyType, BankAccountHolderName, BankAccountNumber, BSBNumber, Eyes,
	SexualPreferences AS SexualPreferencesId, Category AS CategoryId, [Language] AS LanguageId FROM EscortDetail WHERE EscortID = @EscortID

	SELECT Id,EscortID,Duration,InCallRate,OutCallRate FROM EscortRates WHERE EscortID = @EscortID

	SELECT ID,EscortID,FromTime,ToTime,DayNumber,IsNotAvailable,IsAvailable24X7 FROM AvailabilityCalendar WHERE EscortID = @EscortID

	SELECT Id,EscortID,AddressType,[State],City FROM EscortLocation WHERE EscortID = @EscortID

	SELECT ID,EscortID,[FileName],MediaType FROM EscortGallery WHERE EscortID = @EscortID

END
GO

