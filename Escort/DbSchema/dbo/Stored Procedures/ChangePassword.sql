-- =======================================================        
-- Author:  Khushi Saini    
-- Create date: 06-Oct.-2022        
-- Description: Update User Password    
-- Updated By - Karan - 09-Oct-23    
-- =======================================================    
CREATE   PROCEDURE [dbo].[ChangePassword](@UserId int, @Password VARCHAR(200))    
AS    
/*    
 EXEC ChangePassword 1, '2Wg4RjEcLIUDiT9O7m5P9AV5Law=', 'fdgffc'  
 SELECT * FROM [UserDetail]  
*/    
BEGIN
	UPDATE UserDetail  
	SET [PasswordHash] = @Password,
	UpdatedOnUTC = GETUTCDATE()
	WHERE UserId = @UserId  
	AND IsActive = 1  
END  
