-- ================================================================
-- Author:  Khushi Saini
-- Create date: 28-Sep.-2023
-- Description: Get status of accesstoken and active, delete status   
-- =================================================================
CREATE   PROCEDURE [dbo].[CheckAccessTokenExists](@AccessToken VARCHAR(100))
AS
/*
	EXEC CheckAccessTokenExists '583bed8b-941e-43bb-89dd-a26f853138f5'
*/
BEGIN		
	SET NOCOUNT ON;
	
	SELECT 1 as IsTokenExists, IsActive, IsDeleted
	FROM UserDetail
	WITH(NOLOCK)
	WHERE AccessToken = @AccessToken 
END
