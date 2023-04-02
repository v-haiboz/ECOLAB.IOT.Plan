/****** Object:  Table [dbo].[ELinkServerDBMappingTable]    Script Date: 2023/4/1 17:50:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ELinkServerDBMappingTable](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ServerName] [nvarchar](200) NOT NULL,
	[DBName] [nvarchar](200) NOT NULL,
	[TableName] [nvarchar](200) NOT NULL,
	[ColumnName] [nvarchar](100) NOT NULL,
	[Status] int NOT NULL DEFAULT(0),
	[UpdatedAt] [datetime2](7) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_ELinkServerDBMappingTable] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
