USE [PartnerDB]
GO

/****** Object:  Table [dbo].[MessageDeliveredWaiting]    Script Date: 5/20/2020 7:55:59 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MessageDeliveredWaiting](
	[DeliveredGuid] [uniqueidentifier] NOT NULL,
	[SmsInGuid] [uniqueidentifier] NOT NULL,
	[Subscriber] [varchar](15) NOT NULL,
	[Message] [nvarchar](160) NOT NULL,
	[ShortCode] [varchar](15) NOT NULL,
	[ReceivedTime] [datetime] NOT NULL,
	[OperatorId] [int] NOT NULL,
	[Status] [varchar](50) NULL,
	[CooperateId] [int] NULL,
 CONSTRAINT [PK_MessageDeliveredWaiting] PRIMARY KEY CLUSTERED 
(
	[DeliveredGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO