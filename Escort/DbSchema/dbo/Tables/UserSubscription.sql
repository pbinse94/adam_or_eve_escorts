CREATE TABLE [dbo].[UserSubscription]
(
	Id int NOT NULL PRIMARY KEY IDENTITY(1,1)			
,UserId int NOT NULL Foreign Key References UserDetail(UserId)
,SubscriptionId tinyint NOT NULL
,Price decimal(6,2) NOT NULL
,TransactionId varchar(50) NOT NULL
,PurchaseDateUTC datetime
,ExpiryDateUTC datetime
,IsActive bit NOT NULL DEFAULT(1)
,IsCanceled bit
,AddedOnUTC datetime NOT NULL
,UpdatedOnUTC datetime
)
