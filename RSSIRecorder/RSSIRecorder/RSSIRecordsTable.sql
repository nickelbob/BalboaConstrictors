USE [RSSIRecorder]
GO

/****** Object:  Table [dbo].[RSSIRecords]    Script Date: 03/05/2013 22:18:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[RSSIRecords](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SSID] [nvarchar](500) NOT NULL,
	[SignalStrength] [int] NOT NULL,
	[CreatedOnUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_RSSIRecords] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

