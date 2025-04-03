-- =======================================================          
-- Author:  Khushi Saini      
-- Create date: 22-Sep-2022          
-- Description: Update Admin password  
-- Updated By : Karan baghel - 09-Oct-23  
-- =======================================================      
CREATE   PROCEDURE [dbo].[UpdateUserByToken](@ForgotPasswordToken VARCHAR(50), @Password VARCHAR(200))      
AS      
/*      
 EXEC UpdateUserByToken 'e82e93fb-007a-4255-88c0-31b07ed8baa6', 'a4c4jv8rbJIWHkiETIfzkPNrETU6IA=='      
 SELECT * FROM [UserDetail]    
*/      
BEGIN
	 UPDATE UserDetail    
	 SET [PasswordHash] = @Password,    
	  ResetPasswordToken = NULL,
	  UpdatedOnUTC = GETUTCDATE()
	 WHERE ResetPasswordToken = @ForgotPasswordToken    
	 AND IsActive = 1
END 

