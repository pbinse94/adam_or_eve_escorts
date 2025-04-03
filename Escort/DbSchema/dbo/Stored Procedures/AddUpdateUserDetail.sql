-- =============================================          
-- Author:  Khushi Saini    
-- Create date: 03-Oct.-2023          
-- Description: Update User Details    
-- =============================================      
CREATE   PROCEDURE [dbo].[AddUpdateUserDetail]
(		 @Id INT = null,    
         @FirstName VARCHAR(100) = null,   
         @LastName VARCHAR(100) = null,  
		 @PasswordHash VARCHAR(200),
         @PhoneNumber VARCHAR(20) = null, 
		 @UserType TINYINT,
         @IsActive bit = null,  
         @IsDeleted bit = null,  
         --@Token VARCHAR(100) = null,  --set email verification token by UserId    
         @Email VARCHAR(100) = null,     
         @ResetPasswordToken VARCHAR(100) = null,     
         @EmailVerifiedToken VARCHAR(100) = null,  
         @IsEmailVerified bit = null,  
         @ProfileImage varchar(200) = null,  --verify email by check verification token
		 @AccessToken VARCHAR(100) = null,
		 @DeviceToken VARCHAR(100) = null,
		 @DeviceType tinyint = null,
		 @ForgotPasswordDateUTC datetime = null,
		 @AddedOnUTC datetime,
		 @UpdatedOnUTC datetime
)
AS  
/*    
 EXEC AddUpdateUserDetail null, null, null, null, null, null, null, '62b33ca8-a9e9-40d6-8934-91bcb775dd57'  
   
 SELECT * FROM UserDetail order by userid desc  
  
 update userdetail  
 set emailverifiedtoken = '62b33ca8-a9e9-40d6-8934-91bcb775dd57', isemailverified = 0  
 where userid = 2044  
  
*/    
BEGIN    
		IF @Email != ''--Only to set reset password token by email  
		BEGIN
			SELECT TOP 1   
			   @Id = UserId   
			FROM UserDetail   
			WHERE Email = @Email and IsDeleted = 0
		END

		IF @EmailVerifiedToken != ''
		BEGIN
			SELECT TOP 1   
			   @Id = UserId   
			FROM UserDetail   
			WHERE EmailVerifiedToken = @EmailVerifiedToken and IsDeleted = 0  
		END

		IF @Id > 0
		BEGIN
			UPDATE UserDetail  
				 SET [FirstName] = ISNULL(@FirstName, [FirstName]),    
				  [LastName] = ISNULL(@LastName, [LastName]),   
				 PhoneNumber = ISNULL(@PhoneNumber, PhoneNumber),    
				 Email =  ISNULL(@Email, [Email]),
				 IsActive = ISNULL(@IsActive, IsActive),  
				 IsDeleted = ISNULL(@IsDeleted, IsDeleted),  
				 EmailVerifiedToken = CASE WHEN @EmailVerifiedToken IS NOT NULL AND IsEmailVerified = 1 THEN NULL ELSE ISNULL(@EmailVerifiedToken, EmailVerifiedToken) END,    
				 IsEmailVerified = CASE WHEN @EmailVerifiedToken IS NOT NULL AND @EmailVerifiedToken = EmailVerifiedToken THEN 1 ELSE IsEmailVerified END,    
				 ResetPasswordToken = @ResetPasswordToken,    
				 ForgotPasswordDateUTC = CASE WHEN @ResetPasswordToken IS NOT NULL THEN GETUTCDATE() ELSE ForgotPasswordDateUTC END,  
				 ProfileImage = ISNULL(@ProfileImage, ProfileImage), 
				 AccessToken = ISNULL(@AccessToken,AccessToken),
				 UpdatedOnUTC = @UpdatedOnUTC  
			WHERE UserId = @Id  
		END
		ELSE
		BEGIN
			INSERT INTO UserDetail([FirstName],LastName, Email, PasswordHash, AddedOnUTC, IsActive, IsDeleted, UserType, IsEmailVerified, EmailVerifiedToken)      
    VALUES(@FirstName, @LastName, @Email, @PasswordHash, @AddedOnUTC, 1, 0, @UserType, 0, @EmailVerifiedToken)
    
		END
	 
 
END  
  
   