USE [TeamManage_Vote]
GO

/****** Object:  Table [dbo].[VoteProject]    Script Date: 08/27/2014 17:04:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[VoteProject](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Term] [nvarchar](50) NULL,
	[State] [int] NULL,
	[BeginTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[CreatedTime] [datetime] NULL,
	[CreatedBy] [int] NULL,
 CONSTRAINT [PK_VoteProject] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


------------------------------------------------------------------------------------------------------------------


USE [TeamManage_Vote]
GO

/****** Object:  Table [dbo].[VoteItem]    Script Date: 08/27/2014 17:05:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[VoteItem](
	[Id] [int] NOT NULL,
	[PId] [int] NULL,
	[Name] [nvarchar](50) NULL,
	[Nominees] [nvarchar](50) NULL,
	[Nominator] [int] NULL,
	[Comment] [nvarchar](1000) NULL,
	[State] [int] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedBy] [int] NULL,
	[CreatedTime] [datetime] NULL,
	[ModifiedTime] [datetime] NULL,
 CONSTRAINT [PK_VoteItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


------------------------------------------------------------------------------------------------------------------


USE [TeamManage_Vote]
GO

/****** Object:  Table [dbo].[VoteDetail]    Script Date: 08/27/2014 17:05:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[VoteDetail](
	[Id] [int] NOT NULL,
	[PId] [int] NULL,
	[IId] [int] NULL,
	[Voter] [int] NULL,
	[State] [int] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedBy] [int] NULL,
	[CreatedTime] [datetime] NULL,
	[ModifiedTime] [datetime] NULL,
 CONSTRAINT [PK_VoteDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

