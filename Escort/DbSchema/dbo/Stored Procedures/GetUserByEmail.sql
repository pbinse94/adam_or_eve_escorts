-- ==========================================================          
-- Author:  Satish Chandra Sudhansu          
-- Create date: 06-Sep.-2022          
-- Description: This Procedure is used for check user exists.
-- ==========================================================          
CREATE   PROCEDURE [dbo].[GetUserByEmail] ( @Email VARCHAR(100) )           
AS          
/*          
 EXEC GetUserByEmail 'sdfdsf'          
 SELECT * FROM UserDetail         
*/          
BEGIN          
 SET NOCOUNT ON;    
      
  SELECT UserId, PasswordHash, [FirstName], LastName, Email, UserType,    
  ProfileImage, IsActive,  IsEmailVerified, ForgotPasswordDateUTC,AccessToken    
  FROM UserDetail 
  WITH(NOLOCK)
  WHERE Email = @Email AND IsDeleted = 0    
END 
