-- ====================================================================  
-- Author:  Khushi Saini  
-- Create date: 22-Sep-2023      
-- Description: Check Forgot Password Token is exists or not  for admin  
-- Updated By : Karan baghel - 09-Oct-23
-- ====================================================================  
CREATE   PROCEDURE [dbo].[CheckResetPasswordTokenExistByToken](@Token VARCHAR(100))  
/*  
 EXEC CheckResetPasswordTokenExistByToken '7dcb6207-172a-46b2-8828-3f1c0b3e849a'  
*/  
AS  
BEGIN  
	SET NOCOUNT ON;

	SELECT 1 AS Result
	FROM UserDetail
	WITH(NOLOCK)
	WHERE ResetPasswordToken = @Token
	AND IsActive = 1
	AND IsDeleted = 0;
END  
