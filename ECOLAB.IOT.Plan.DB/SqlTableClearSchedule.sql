/****** Object:  Table [dbo].[SqlTableClearSchedule]    Script Date: 2023/3/9 16:02:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SqlTableClearSchedule](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SqlServerId] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[JObject] [nvarchar](200) NOT NULL,
	[PartitionKey] [nvarchar](200) NOT NULL,
	[Enable] [bit] NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_SqlServerRemoveSchedule] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1 表示Custom,2 表示Dynamic' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SqlTableClearSchedule', @level2type=N'COLUMN',@level2name=N'Type'
GO