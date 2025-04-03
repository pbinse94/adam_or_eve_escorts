-- ======================================================================        
-- Author:  Anirudh Singh Rathore     
-- Create date: 21.08.2023        
-- Description: This Procedure is used for update forget password token      
-- ======================================================================       
CREATE   PROCEDURE [dbo].[SaveResetTokenByEmail] (@Email VARCHAR(100), @Token VARCHAR(100))
AS      
/*      
 EXEC SaveForgetPasswordToken  '',''      
 SELECT * FROM [UserDetail]      
*/      
BEGIN      
	SET NOCOUNT ON;
	
	UPDATE UserDetail
	SET ResetPasswordToken = @Token,
		ForgotPasswordDateUTC = GETUTCDATE()
	WHERE Email = @Email
END


 
