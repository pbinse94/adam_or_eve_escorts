-- =================================================================   
-- Author: Khushi Saini
-- Create date: 13-Oct.-2023
-- Description: Get user details for reset password with reset token
-- =================================================================
CREATE   PROCEDURE [dbo].[GetUserDetailByToken](@Token varchar(100))    
AS        
/*        
 EXEC [GetUserDetailByToken] 'dgkfjdg,g'     
 SELECT * FROM UserDetail
*/        
BEGIN        
 SET NOCOUNT ON;    
      
 SELECT UserId, [FirstName], LastName, Email, UserType, PhoneNumber,  
 IsEmailVerified, PasswordHash,    
 ProfileImage, IsActive, IsDeleted  
 FROM UserDetail 
 WITH(NOLOCK)
 WHERE ResetPasswordToken = @Token
 END 
