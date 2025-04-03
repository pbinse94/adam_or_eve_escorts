-- ===============================================    
-- Author:  Khushi Saini    
-- Create date: 28-Sep.-2023    
-- Description: Manage Device Token   
-- Updated By : Karan baghel - 09-Oct-23  
-- ===============================================    
CREATE   PROCEDURE [dbo].[ManageDeviceToken] ( @Email VARCHAR(100),    
           @DeviceType TINYINT,    
           @DeviceToken VARCHAR(200),    
           @AccessToken VARCHAR(100))    
               
AS        
/*    
	EXEC ManageDeviceToken '', '', '', ''
*/    
BEGIN       
	UPDATE UserDetail
	SET DeviceType = @DeviceType,
	DeviceToken = @DeviceToken,
	AccessToken = @AccessToken
	WHERE Email = @Email AND IsDeleted = 0 AND IsActive = 1
END
