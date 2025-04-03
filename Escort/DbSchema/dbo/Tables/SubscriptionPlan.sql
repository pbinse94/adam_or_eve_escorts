CREATE TABLE [dbo].[SubscriptionPlan]
(
	ID tinyint PRIMARY KEY IDENTITY(1,1),
	Title varchar(20) NOT NULL,
	[Description] varchar(1000) NOT NULL,
	PlanType tinyint,
	PriceId varchar(100),
	DisplayPrice decimal(6,2),
	IsDeleted bit DEFAULT(0) NOT NULL,
	AddedOnUTC datetime,
	UpdatedOnUTC datetime
)
