-- ========================================================        
-- Author: Khushi Saini        
-- Create date: 22-Sep.-2023        
-- Description: Forgot password token update for Web Users      
-- Updated By : Karan baghel - 09-Oct-23    
-- ========================================================        
CREATE   PROCEDURE [dbo].[UpdateForgotPasswordToken](@UserId BIGINT, @ForgotPasswordToken VARCHAR(100))        
AS        
/*        
 EXEC UpdateForgotPasswordToken 19, 'fiuydeitdhtgi'        
 SELECT * FROM UserDetail  
      
*/        
BEGIN        
	SET NOCOUNT ON;  
    
	DECLARE @TmpTbl TABLE(UserId INT, Email VARCHAR(150), UserName VARCHAR(150), ForgotPasswordToken VARCHAR(150),     
		IsActive BIT, IsDeleted BIT, IsEmailVerified BIT,  ForgotPasswordDateUTC DATETIME)    
	DECLARE @IsValid tinyint = 0;        
    
	INSERT INTO @TmpTbl (UserId, Email,UserName, ForgotPasswordToken, IsActive, IsDeleted, IsEmailVerified,ForgotPasswordDateUTC)    
	SELECT UserId, Email, [FirstName] + ' ' + [LastName] UserName, ResetPasswordToken ForgotPasswordToken, IsActive, IsDeleted, IsEmailVerified, ForgotPasswordDateUTC        
	FROM UserDetail    
	WITH(NOLOCK)    
	WHERE UserId = @UserId     
  
	IF EXISTS( SELECT UserId         
	FROM @TmpTbl        
	WHERE UserId = @UserId AND IsActive = 1 AND IsDeleted = 0         
	AND IsEmailVerified = 1         
	AND (ForgotPasswordToken IS NULL         
	OR DATEDIFF(MINUTE, ForgotPasswordDateUTC, GETUTCDATE()) > 30))        
	BEGIN        
		UPDATE UserDetail        
		SET ResetPasswordToken = @ForgotPasswordToken,        
		ForgotPasswordDateUTC = GETUTCDATE(),        
		@IsValid = 1,
		UpdatedOnUTC = GETUTCDATE()
		WHERE UserId = @UserId        
	END        
          
	ELSE IF EXISTS( SELECT UserId         
		FROM @TmpTbl         
		WHERE UserId = @UserId AND IsActive = 1 AND IsDeleted = 0 AND IsEmailVerified = 1)        
	BEGIN        
		SET @IsValid = 2 -- Token not update as, Already Sent forgot password mail within 30 minutes.        
	END        
          
	ELSE        
	BEGIN        
		SET @IsValid = 0 -- Token not updated.        
	END      
    
	SELECT UserId, Email,  UserName,  @ForgotPasswordToken AS ForgotPasswordToken, IsActive, IsDeleted, IsEmailVerified, @IsValid AS IsValid        
	FROM @TmpTbl       
END 
