CREATE TABLE [dbo].[UserDetail] (
    [UserId]                INT           IDENTITY (1, 1) NOT NULL,
    [Email]                 VARCHAR (100) NOT NULL,
    [PasswordHash]          VARCHAR (200) NOT NULL,
    [UserType]              TINYINT       NOT NULL,
    [FirstName]                  VARCHAR (50) NULL,
    [LastName]                  VARCHAR (50) NULL,
    [DisplayName]                  VARCHAR (110) NULL,
    [PhoneNumber]           VARCHAR (20)  NULL,
    [ProfileImage]          VARCHAR (200) NULL,
    [IsActive]              BIT           CONSTRAINT [DF_UserDetail_IsActive] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_UserDetail_IsDeleted] DEFAULT ((0)) NOT NULL,
    [IsEmailVerified]       BIT           CONSTRAINT [DF_UserDetail_IsEmailVerified] DEFAULT ((0)) NOT NULL,
    [EmailVerifiedToken]    VARCHAR (100) NULL,
    [ResetPasswordToken]    VARCHAR (100) NULL,
    [AccessToken]           VARCHAR (100) NULL,
    [DeviceType]            TINYINT       NULL,
    [DeviceToken]           VARCHAR (200) NULL,
    [ForgotPasswordDateUTC] DATETIME      NULL,
    [AddedOnUTC]            DATETIME2 (7) CONSTRAINT [DF_UserDetail_AddedOnUTC] DEFAULT (getutcdate()) NOT NULL,
    [UpdatedOnUTC]          DATETIME2 (7) NULL,
    CONSTRAINT [PK_UserDetail] PRIMARY KEY CLUSTERED ([UserId] ASC)
);

