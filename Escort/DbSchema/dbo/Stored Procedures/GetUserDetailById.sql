-- ==========================================================            
-- Author:          
-- Create date:        
-- Description: This Procedure is used for get data of user.    
-- Updated By : Karan baghel - 10-Oct-23  
-- ==========================================================       
CREATE PROCEDURE [dbo].[GetUserDetailById](@Id INT)      
AS          
/*          
 EXEC GetUserDetailById 4046       
 SELECT * FROM UserDetails     
*/          
BEGIN          
 SET NOCOUNT ON;      
        
 SELECT UserId AS Id, [FirstName], LastName, Email, UserType, PhoneNumber,  
  IsEmailVerified, PasswordHash, AccessToken,     
  ProfileImage, IsActive, IsDeleted, DisplayName    
 FROM UserDetail   
 WITH(NOLOCK)  
 WHERE UserId = @Id     
 END   