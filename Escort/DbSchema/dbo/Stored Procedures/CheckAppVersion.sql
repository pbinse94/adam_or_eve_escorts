-- ===============================================
-- Author:  Khushi Saini
-- Create date: 28-Sep.-2023
-- Description: Check App Version is latest or not
-- ===============================================
CREATE  PROCEDURE [dbo].[CheckAppVersion](@AppVersion VARCHAR(50),@DeviceTypeId tinyint)
AS
/*
	EXEC CheckAppVersion '1'
	SELECT * FROM UserDetails
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @InComingAppVersionId int, @ForceUpdateAppVersionId int

	SET @InComingAppVersionId =   (SELECT TOP 1 Id 
							FROM AppVersion 
							WITH(NOLOCK)
							WHERE VersionNumber = @AppVersion AND DeviceTypeId = @DeviceTypeId
							)

	SET @ForceUpdateAppVersionId =   (SELECT TOP 1 Id 
		FROM AppVersion 
		WITH(NOLOCK)
		WHERE Id > @InComingAppVersionId AND IsActive = 1 AND ForceUpdate = 1 AND DeviceTypeId = @DeviceTypeId
		)
	

	IF (@ForceUpdateAppVersionId > @InComingAppVersionId)
		SELECT 1 As Result
	ELSE
		SELECT 0 As Result
END
