CREATE TABLE [dbo].[AppVersion] (
    [Id]            INT          IDENTITY (1, 1) NOT NULL,
    [VersionNumber] VARCHAR (50) NOT NULL,
    [DeviceTypeId]  TinyInt      NULL,
    [ForceUpdate]   BIT          NOT NULL Default(0),
    [IsActive]   BIT NOT NULL Default(1),
    [CreatedOn]     DATETIME     NOT NULL,
    CONSTRAINT [PK_AppVersion] PRIMARY KEY CLUSTERED ([Id] ASC)
);

