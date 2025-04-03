CREATE TABLE [dbo].[StateTimezone](
	[StateTimezoneId] [int] IDENTITY(1,1) NOT NULL,
	[StateAbbreviation] [char](2) NOT NULL,
	[WindowsTimezone] [varchar](30) NOT NULL,
	[TimezoneLookUpId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[StateTimezoneId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[StateTimezone] ON 

INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (1, N'AL', N'Central Standard Time', 10)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (2, N'AR', N'Central Standard Time', 10)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (3, N'IL', N'Central Standard Time', 10)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (4, N'IA', N'Central Standard Time', 10)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (5, N'KS', N'Central Standard Time', 10)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (6, N'LA', N'Central Standard Time', 10)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (7, N'MN', N'Central Standard Time', 10)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (8, N'MS', N'Central Standard Time', 10)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (9, N'MO', N'Central Standard Time', 10)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (10, N'NE', N'Central Standard Time', 10)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (11, N'ND', N'Central Standard Time', 10)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (12, N'OK', N'Central Standard Time', 10)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (13, N'SD', N'Central Standard Time', 10)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (14, N'TN', N'Central Standard Time', 10)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (15, N'TX', N'Central Standard Time', 10)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (16, N'WI', N'Central Standard Time', 10)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (17, N'AK', N'Alaska Standard Time', NULL)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (18, N'AZ', N'Mountain Standard Time', 9)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (19, N'CO', N'Mountain Standard Time', 9)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (20, N'ID', N'Mountain Standard Time', 9)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (21, N'MT', N'Mountain Standard Time', 9)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (22, N'NM', N'Mountain Standard Time', 9)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (23, N'UT', N'Mountain Standard Time', 9)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (24, N'WY', N'Mountain Standard Time', 9)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (25, N'CA', N'Pacific Standard Time', 6)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (26, N'NV', N'Pacific Standard Time', 6)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (27, N'OR', N'Pacific Standard Time', 6)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (28, N'WA', N'Pacific Standard Time', 6)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (29, N'CT', N'Eastern Standard Time', 15)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (30, N'DE', N'Eastern Standard Time', 15)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (31, N'DC', N'Eastern Standard Time', 15)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (32, N'FL', N'Eastern Standard Time', 15)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (33, N'GA', N'Eastern Standard Time', 15)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (34, N'IN', N'Eastern Standard Time', 15)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (35, N'KY', N'Eastern Standard Time', 15)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (36, N'ME', N'Eastern Standard Time', 15)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (37, N'MD', N'Eastern Standard Time', 15)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (38, N'MA', N'Eastern Standard Time', 15)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (39, N'MI', N'Eastern Standard Time', 15)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (40, N'NH', N'Eastern Standard Time', 15)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (41, N'NJ', N'Eastern Standard Time', 15)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (42, N'NY', N'Eastern Standard Time', 15)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (43, N'NC', N'Eastern Standard Time', 15)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (44, N'OH', N'Eastern Standard Time', 15)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (45, N'PA', N'Eastern Standard Time', 15)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (46, N'RI', N'Eastern Standard Time', 15)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (47, N'SC', N'Eastern Standard Time', 15)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (48, N'VT', N'Eastern Standard Time', 15)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (49, N'VA', N'Eastern Standard Time', 15)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (50, N'WV', N'Eastern Standard Time', 15)
INSERT [dbo].[StateTimezone] ([StateTimezoneId], [StateAbbreviation], [WindowsTimezone], [TimezoneLookUpId]) VALUES (51, N'HI', N'Hawaiian Standard Time', 3)
SET IDENTITY_INSERT [dbo].[StateTimezone] OFF
GO