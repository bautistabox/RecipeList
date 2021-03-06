USE [RecipesLists]
GO
/****** Object:  Table [dbo].[categories]    Script Date: 2/28/2019 8:21:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[categories](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_categories] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[email_verification]    Script Date: 2/28/2019 8:21:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[email_verification](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[guid] [uniqueidentifier] NOT NULL,
	[is_verified] [bit] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[expired_guids]    Script Date: 2/28/2019 8:21:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[expired_guids](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[expired_guid] [uniqueidentifier] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ingredients]    Script Date: 2/28/2019 8:21:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ingredients](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[created_at] [date] NOT NULL,
 CONSTRAINT [PK_ingredients] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[list_items]    Script Date: 2/28/2019 8:21:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[list_items](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[list_id] [int] NOT NULL,
	[item_name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_list_items] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[lists]    Script Date: 2/28/2019 8:21:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[lists](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[user_id] [int] NOT NULL,
	[created_at] [date] NOT NULL,
	[updated_at] [datetime2](7) NULL,
 CONSTRAINT [PK_lists] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ratings]    Script Date: 2/28/2019 8:21:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ratings](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[rating] [tinyint] NOT NULL,
	[comments] [text] NOT NULL,
	[recipe_id] [int] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[recipe_ingredients]    Script Date: 2/28/2019 8:21:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[recipe_ingredients](
	[ingredient_id] [int] NOT NULL,
	[recipe_id] [int] NOT NULL,
	[amount] [varchar](20) NULL,
	[amount_type] [varchar](20) NULL,
 CONSTRAINT [PK_recipe_ingredients] PRIMARY KEY CLUSTERED 
(
	[ingredient_id] ASC,
	[recipe_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[recipes]    Script Date: 2/28/2019 8:21:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[recipes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[description] [text] NOT NULL,
	[instruction] [text] NOT NULL,
	[category_id] [int] NOT NULL,
	[photo] [varbinary](max) NULL,
	[uploader_id] [int] NOT NULL,
	[created_at] [datetime2](7) NOT NULL,
	[updated_at] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_recipes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users]    Script Date: 2/28/2019 8:21:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[email] [varchar](255) NOT NULL,
	[username] [varchar](50) NOT NULL,
	[display_name] [varchar](50) NOT NULL,
	[password] [varchar](64) NOT NULL,
	[registered_at] [datetime2](7) NOT NULL,
	[last_login_at] [datetime2](7) NULL,
 CONSTRAINT [PK_users] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[list_items]  WITH CHECK ADD  CONSTRAINT [FK_list_items_lists] FOREIGN KEY([list_id])
REFERENCES [dbo].[lists] ([id])
GO
ALTER TABLE [dbo].[list_items] CHECK CONSTRAINT [FK_list_items_lists]
GO
ALTER TABLE [dbo].[lists]  WITH CHECK ADD  CONSTRAINT [FK_lists_users] FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[lists] CHECK CONSTRAINT [FK_lists_users]
GO
ALTER TABLE [dbo].[recipe_ingredients]  WITH CHECK ADD  CONSTRAINT [FK_recipe_ingredients_ingredients] FOREIGN KEY([ingredient_id])
REFERENCES [dbo].[ingredients] ([id])
GO
ALTER TABLE [dbo].[recipe_ingredients] CHECK CONSTRAINT [FK_recipe_ingredients_ingredients]
GO
ALTER TABLE [dbo].[recipe_ingredients]  WITH CHECK ADD  CONSTRAINT [FK_recipe_ingredients_recipes] FOREIGN KEY([recipe_id])
REFERENCES [dbo].[recipes] ([id])
GO
ALTER TABLE [dbo].[recipe_ingredients] CHECK CONSTRAINT [FK_recipe_ingredients_recipes]
GO
ALTER TABLE [dbo].[recipes]  WITH CHECK ADD  CONSTRAINT [FK_recipes_categories] FOREIGN KEY([category_id])
REFERENCES [dbo].[categories] ([id])
GO
ALTER TABLE [dbo].[recipes] CHECK CONSTRAINT [FK_recipes_categories]
GO
ALTER TABLE [dbo].[recipes]  WITH CHECK ADD  CONSTRAINT [FK_recipes_users] FOREIGN KEY([uploader_id])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[recipes] CHECK CONSTRAINT [FK_recipes_users]
GO
