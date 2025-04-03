CREATE PROCEDURE [dbo].[EditProfile]
(
	@EscortID INT,
    @UserId INT,
	@ProfileImage VARCHAR(200),
    @FirstName VARCHAR(50),
    @LastName VARCHAR(50),
    @Email VARCHAR(100),
    @PhoneNumber VARCHAR(20),
    @DisplayName VARCHAR(100) = null,
    @Age INT = null,
    @Height INT = null,
    @Eyes VARCHAR(20) = null,
    @Category VARCHAR(50) = null,
    @BodyType VARCHAR(50) = null,
    @Language VARCHAR(50) = null,
    @Bio VARCHAR(MAX) = null,
	@SexualPreferences VARCHAR(50) = null,
	@LocationTableType LocationTableType READONLY,
	@EscortRatesTableType EscortRatesTableType READONLY,
	@AvailabilityCalendarTableType AvailabilityCalendarTableType READONLY
)
AS
BEGIN
    SET NOCOUNT ON;

	DECLARE @newEscortId INT  
    DECLARE @ExistingUserId INT;

    -- Check if the user already exists
    SELECT @ExistingUserId = EscortID
    FROM EscortDetail
    WHERE UserId = @UserId;

	-- Escort details
    IF @ExistingUserId IS NOT NULL
    BEGIN
		UPDATE EscortDetail
        SET Age = @Age,            
            Height = @Height,
            Eyes = @Eyes,
            Category = @Category,
            BodyType = @BodyType,
            [Language] = @Language,
            Bio = @Bio,
            SexualPreferences = @SexualPreferences
        WHERE UserId = @UserId
		SET @newEscortId = @ExistingUserId
    END
		ELSE
	BEGIN
		INSERT INTO EscortDetail (UserId, Age, Height, Eyes, Category, BodyType, [Language], Bio, SexualPreferences)      
		VALUES (@UserId, @Age, @Height, @Eyes, @Category, @BodyType, @Language, @Bio, @SexualPreferences)
		SET @newEscortId = SCOPE_IDENTITY()
	END

	-- Locations
	DELETE FROM [dbo].[EscortLocation] WHERE EscortID = @EscortID

	INSERT INTO [dbo].[EscortLocation](EscortID, AddressType, [State], City)	   
    SELECT @newEscortId, AddressType, [State], City FROM @LocationTableType

	-- Rates
	DELETE FROM EscortRates WHERE EscortID = @EscortID

	INSERT INTO EscortRates(EscortId, Duration, InCallRate, OutCallRate)	   
    SELECT @newEscortId, Duration, InCallRate, OutCallRate FROM  @EscortRatesTableType

	-- Availability
	DELETE FROM [dbo].[AvailabilityCalendar] WHERE EscortID = @EscortID

	INSERT INTO [AvailabilityCalendar](EscortID, FromTime, ToTime, DayNumber, IsNotAvailable, IsAvailable24X7)	   
    SELECT @newEscortId, FromTime, ToTime, DayNumber, IsNotAvailable, IsAvailable24X7 FROM  @AvailabilityCalendarTableType

	-- User details
	UPDATE UserDetail
    SET FirstName = @FirstName,
        LastName = @LastName,
        DisplayName = @DisplayName,
		PhoneNumber = @PhoneNumber,
		ProfileImage = @ProfileImage
    WHERE UserId = @UserId

	SELECT @newEscortId
END
