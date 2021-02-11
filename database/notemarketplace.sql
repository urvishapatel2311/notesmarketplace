USE [master]
GO
/****** Object:  Database [notemarketplace]    Script Date: 11-02-2021 18:32:19 ******/
CREATE DATABASE [notemarketplace]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'notemarketplace', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\notemarketplace.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'notemarketplace_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\notemarketplace_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [notemarketplace] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [notemarketplace].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [notemarketplace] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [notemarketplace] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [notemarketplace] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [notemarketplace] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [notemarketplace] SET ARITHABORT OFF 
GO
ALTER DATABASE [notemarketplace] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [notemarketplace] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [notemarketplace] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [notemarketplace] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [notemarketplace] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [notemarketplace] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [notemarketplace] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [notemarketplace] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [notemarketplace] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [notemarketplace] SET  DISABLE_BROKER 
GO
ALTER DATABASE [notemarketplace] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [notemarketplace] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [notemarketplace] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [notemarketplace] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [notemarketplace] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [notemarketplace] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [notemarketplace] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [notemarketplace] SET RECOVERY FULL 
GO
ALTER DATABASE [notemarketplace] SET  MULTI_USER 
GO
ALTER DATABASE [notemarketplace] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [notemarketplace] SET DB_CHAINING OFF 
GO
ALTER DATABASE [notemarketplace] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [notemarketplace] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [notemarketplace] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'notemarketplace', N'ON'
GO
ALTER DATABASE [notemarketplace] SET QUERY_STORE = OFF
GO
USE [notemarketplace]
GO
/****** Object:  Table [dbo].[downloads]    Script Date: 11-02-2021 18:32:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[downloads](
	[id] [int] NOT NULL,
	[noteID] [int] NOT NULL,
	[seller] [int] NOT NULL,
	[downloader] [int] NOT NULL,
	[isSellerHasAllowedDownload] [bit] NOT NULL,
	[attachementPath] [varchar](max) NULL,
	[attachementDownloadDate] [datetime] NULL,
	[isPaid] [bit] NOT NULL,
	[purchasedPrice] [decimal](10, 5) NULL,
	[noteTitle] [varchar](100) NOT NULL,
	[noteCategory] [varchar](100) NOT NULL,
	[createdDate] [datetime] NOT NULL,
	[createdBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
 CONSTRAINT [PK_downloads] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[manage_country]    Script Date: 11-02-2021 18:32:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[manage_country](
	[id] [int] NOT NULL,
	[addedBy] [int] NOT NULL,
	[countryName] [varchar](100) NOT NULL,
	[countryCode] [varchar](10) NOT NULL,
	[createdDate] [datetime] NULL,
	[createdBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[isActive] [bit] NOT NULL,
 CONSTRAINT [PK_manage_country] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[manage_note_category]    Script Date: 11-02-2021 18:32:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[manage_note_category](
	[id] [int] NOT NULL,
	[addedBy] [int] NOT NULL,
	[categoryName] [varchar](100) NOT NULL,
	[description] [varchar](max) NOT NULL,
	[createdDate] [datetime] NULL,
	[creatdBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[isActive] [bit] NOT NULL,
 CONSTRAINT [PK_manage_note_category] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[manage_note_type]    Script Date: 11-02-2021 18:32:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[manage_note_type](
	[id] [int] NOT NULL,
	[addedBy] [int] NOT NULL,
	[typeName] [varchar](100) NOT NULL,
	[description] [varchar](max) NOT NULL,
	[createdDate] [datetime] NULL,
	[createdBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[isActive] [bit] NOT NULL,
 CONSTRAINT [PK_manage_note_type] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[note_attechment]    Script Date: 11-02-2021 18:32:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[note_attechment](
	[id] [int] NOT NULL,
	[noteID] [int] NOT NULL,
	[fileName] [varchar](100) NOT NULL,
	[filePath] [varchar](max) NOT NULL,
	[createdDate] [datetime] NULL,
	[createdBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[isActive] [bit] NOT NULL,
 CONSTRAINT [PK_note_attechment] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[note_Details]    Script Date: 11-02-2021 18:32:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[note_Details](
	[id] [int] NOT NULL,
	[sellerID] [int] NOT NULL,
	[status] [int] NOT NULL,
	[actionedBy] [int] NOT NULL,
	[adminRemark] [varchar](max) NULL,
	[noteTitle] [varchar](100) NOT NULL,
	[notePicture] [varchar](500) NULL,
	[noteCategory] [int] NOT NULL,
	[noteUniversity] [varchar](200) NULL,
	[noteType] [int] NULL,
	[description] [varchar](max) NOT NULL,
	[numberOfPage] [int] NULL,
	[publishedDate] [datetime] NULL,
	[country] [int] NULL,
	[courseName] [varchar](100) NULL,
	[courseCode] [varchar](100) NULL,
	[professor] [varchar](100) NULL,
	[isPaid] [bit] NOT NULL,
	[sellingPrice] [decimal](18, 5) NULL,
	[notePreview] [nvarchar](max) NULL,
	[createdDate] [datetime] NOT NULL,
	[createdBy] [int] NULL,
	[modifieddate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[isActive] [bit] NOT NULL,
 CONSTRAINT [PK_note_Details] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[note_Reported_Issues]    Script Date: 11-02-2021 18:32:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[note_Reported_Issues](
	[id] [int] NOT NULL,
	[noteID] [int] NOT NULL,
	[reportedByID] [int] NOT NULL,
	[againstDownloadID] [int] NULL,
	[remarks] [varchar](max) NOT NULL,
	[createdDate] [datetime] NOT NULL,
	[createdBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
 CONSTRAINT [PK_note_Reported_Issues] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[notereviews]    Script Date: 11-02-2021 18:32:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[notereviews](
	[id] [int] NOT NULL,
	[noteID] [int] NOT NULL,
	[reviewByID] [int] NOT NULL,
	[againstDownloadID] [int] NOT NULL,
	[rating] [decimal](10, 5) NOT NULL,
	[comments] [varchar](max) NOT NULL,
	[createDate] [datetime] NULL,
	[createdBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[isActive] [bit] NOT NULL,
 CONSTRAINT [PK_notereviews] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[reference_Data]    Script Date: 11-02-2021 18:32:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[reference_Data](
	[id] [int] NOT NULL,
	[value] [varchar](100) NOT NULL,
	[dataValue] [varchar](100) NOT NULL,
	[refCategory] [varchar](100) NOT NULL,
	[createdDate] [datetime] NULL,
	[createdBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[isActive] [bit] NOT NULL,
 CONSTRAINT [PK_reference_Data] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[system_config]    Script Date: 11-02-2021 18:32:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[system_config](
	[id] [int] NOT NULL,
	[supportEmailID] [varchar](100) NOT NULL,
	[emailForEvent] [varchar](100) NOT NULL,
	[fbURL] [varchar](max) NULL,
	[twitterURL] [varchar](max) NULL,
	[linkedinURL] [varchar](max) NULL,
	[defaultNoteImage] [varchar](500) NOT NULL,
	[defaultProfilePicture] [varchar](500) NOT NULL,
	[supportPhoneNo] [varchar](15) NOT NULL,
	[createdDate] [datetime] NULL,
	[createdBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[isActive] [bit] NOT NULL,
 CONSTRAINT [PK_system_config] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[user_profile]    Script Date: 11-02-2021 18:32:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_profile](
	[id] [int] NOT NULL,
	[userID] [int] NOT NULL,
	[gender] [varchar](10) NULL,
	[dob] [datetime] NULL,
	[countryCode_mobileno] [varchar](10) NOT NULL,
	[secondaryEmailID] [varchar](100) NULL,
	[phoneNo] [varchar](20) NOT NULL,
	[profilePicture] [varchar](500) NULL,
	[addressLine1] [varchar](100) NOT NULL,
	[addressLine2] [varchar](100) NULL,
	[city] [varchar](50) NOT NULL,
	[state] [varchar](50) NOT NULL,
	[zipcode] [varchar](50) NOT NULL,
	[country] [varchar](50) NOT NULL,
	[university] [varchar](100) NULL,
	[college] [varchar](100) NULL,
	[createDate] [datetime] NULL,
	[createBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
 CONSTRAINT [PK_user_profile] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[user_roles]    Script Date: 11-02-2021 18:32:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_roles](
	[id] [int] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[description] [varchar](max) NULL,
	[createDate] [datetime] NULL,
	[createBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[isActive] [bit] NOT NULL,
 CONSTRAINT [PK_user_roles] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users]    Script Date: 11-02-2021 18:32:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[id] [int] NOT NULL,
	[roleID] [int] NOT NULL,
	[firstName] [varchar](50) NOT NULL,
	[lastName] [varchar](50) NOT NULL,
	[emailID] [varchar](100) NOT NULL,
	[isEmailVerified] [bit] NOT NULL,
	[password] [varchar](24) NOT NULL,
	[createDate] [datetime] NULL,
	[createBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[isActive] [bit] NOT NULL,
 CONSTRAINT [PK_users] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [note_detail_title_uniquekey]    Script Date: 11-02-2021 18:32:20 ******/
ALTER TABLE [dbo].[note_Details] ADD  CONSTRAINT [note_detail_title_uniquekey] UNIQUE NONCLUSTERED 
(
	[noteTitle] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_system_config]    Script Date: 11-02-2021 18:32:20 ******/
ALTER TABLE [dbo].[system_config] ADD  CONSTRAINT [IX_system_config] UNIQUE NONCLUSTERED 
(
	[emailForEvent] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_system_config_1]    Script Date: 11-02-2021 18:32:20 ******/
ALTER TABLE [dbo].[system_config] ADD  CONSTRAINT [IX_system_config_1] UNIQUE NONCLUSTERED 
(
	[emailForEvent] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_user_profile]    Script Date: 11-02-2021 18:32:20 ******/
ALTER TABLE [dbo].[user_profile] ADD  CONSTRAINT [IX_user_profile] UNIQUE NONCLUSTERED 
(
	[secondaryEmailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_users]    Script Date: 11-02-2021 18:32:20 ******/
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [IX_users] UNIQUE NONCLUSTERED 
(
	[emailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[manage_country] ADD  CONSTRAINT [manage_country_active]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[note_attechment] ADD  CONSTRAINT [note_attechment_active]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[notereviews] ADD  CONSTRAINT [note_review_active]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[reference_Data] ADD  CONSTRAINT [reference_active]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[system_config] ADD  CONSTRAINT [default_active]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[user_roles] ADD  CONSTRAINT [user_role_active]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [user_active]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[downloads]  WITH CHECK ADD  CONSTRAINT [FK_downloads_note_Details] FOREIGN KEY([noteID])
REFERENCES [dbo].[note_Details] ([id])
GO
ALTER TABLE [dbo].[downloads] CHECK CONSTRAINT [FK_downloads_note_Details]
GO
ALTER TABLE [dbo].[downloads]  WITH CHECK ADD  CONSTRAINT [FK_downloads_users] FOREIGN KEY([seller])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[downloads] CHECK CONSTRAINT [FK_downloads_users]
GO
ALTER TABLE [dbo].[downloads]  WITH CHECK ADD  CONSTRAINT [FK_downloads_users1] FOREIGN KEY([downloader])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[downloads] CHECK CONSTRAINT [FK_downloads_users1]
GO
ALTER TABLE [dbo].[manage_country]  WITH CHECK ADD  CONSTRAINT [FK_manage_country_users] FOREIGN KEY([addedBy])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[manage_country] CHECK CONSTRAINT [FK_manage_country_users]
GO
ALTER TABLE [dbo].[manage_note_category]  WITH CHECK ADD  CONSTRAINT [FK_manage_note_category_users] FOREIGN KEY([addedBy])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[manage_note_category] CHECK CONSTRAINT [FK_manage_note_category_users]
GO
ALTER TABLE [dbo].[manage_note_type]  WITH CHECK ADD  CONSTRAINT [FK_manage_note_type_users] FOREIGN KEY([addedBy])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[manage_note_type] CHECK CONSTRAINT [FK_manage_note_type_users]
GO
ALTER TABLE [dbo].[note_attechment]  WITH CHECK ADD  CONSTRAINT [FK_note_attechment_note_Details] FOREIGN KEY([noteID])
REFERENCES [dbo].[note_Details] ([id])
GO
ALTER TABLE [dbo].[note_attechment] CHECK CONSTRAINT [FK_note_attechment_note_Details]
GO
ALTER TABLE [dbo].[note_Details]  WITH CHECK ADD  CONSTRAINT [FK_note_Details_manage_note_category] FOREIGN KEY([noteCategory])
REFERENCES [dbo].[manage_note_category] ([id])
GO
ALTER TABLE [dbo].[note_Details] CHECK CONSTRAINT [FK_note_Details_manage_note_category]
GO
ALTER TABLE [dbo].[note_Details]  WITH CHECK ADD  CONSTRAINT [FK_note_Details_manage_note_type] FOREIGN KEY([noteType])
REFERENCES [dbo].[manage_note_type] ([id])
GO
ALTER TABLE [dbo].[note_Details] CHECK CONSTRAINT [FK_note_Details_manage_note_type]
GO
ALTER TABLE [dbo].[note_Details]  WITH CHECK ADD  CONSTRAINT [FK_note_Details_reference_Data] FOREIGN KEY([status])
REFERENCES [dbo].[reference_Data] ([id])
GO
ALTER TABLE [dbo].[note_Details] CHECK CONSTRAINT [FK_note_Details_reference_Data]
GO
ALTER TABLE [dbo].[note_Details]  WITH CHECK ADD  CONSTRAINT [FK_note_Details_users] FOREIGN KEY([actionedBy])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[note_Details] CHECK CONSTRAINT [FK_note_Details_users]
GO
ALTER TABLE [dbo].[note_Details]  WITH CHECK ADD  CONSTRAINT [FK_note_Details_users1] FOREIGN KEY([sellerID])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[note_Details] CHECK CONSTRAINT [FK_note_Details_users1]
GO
ALTER TABLE [dbo].[note_Reported_Issues]  WITH CHECK ADD  CONSTRAINT [FK_note_Reported_Issues_downloads] FOREIGN KEY([againstDownloadID])
REFERENCES [dbo].[downloads] ([id])
GO
ALTER TABLE [dbo].[note_Reported_Issues] CHECK CONSTRAINT [FK_note_Reported_Issues_downloads]
GO
ALTER TABLE [dbo].[note_Reported_Issues]  WITH CHECK ADD  CONSTRAINT [FK_note_Reported_Issues_note_Details] FOREIGN KEY([noteID])
REFERENCES [dbo].[note_Details] ([id])
GO
ALTER TABLE [dbo].[note_Reported_Issues] CHECK CONSTRAINT [FK_note_Reported_Issues_note_Details]
GO
ALTER TABLE [dbo].[note_Reported_Issues]  WITH CHECK ADD  CONSTRAINT [FK_note_Reported_Issues_users] FOREIGN KEY([reportedByID])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[note_Reported_Issues] CHECK CONSTRAINT [FK_note_Reported_Issues_users]
GO
ALTER TABLE [dbo].[notereviews]  WITH CHECK ADD  CONSTRAINT [FK_notereviews_downloads] FOREIGN KEY([againstDownloadID])
REFERENCES [dbo].[downloads] ([id])
GO
ALTER TABLE [dbo].[notereviews] CHECK CONSTRAINT [FK_notereviews_downloads]
GO
ALTER TABLE [dbo].[notereviews]  WITH CHECK ADD  CONSTRAINT [FK_notereviews_note_Details] FOREIGN KEY([noteID])
REFERENCES [dbo].[note_Details] ([id])
GO
ALTER TABLE [dbo].[notereviews] CHECK CONSTRAINT [FK_notereviews_note_Details]
GO
ALTER TABLE [dbo].[notereviews]  WITH CHECK ADD  CONSTRAINT [FK_notereviews_users] FOREIGN KEY([reviewByID])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[notereviews] CHECK CONSTRAINT [FK_notereviews_users]
GO
ALTER TABLE [dbo].[system_config]  WITH CHECK ADD  CONSTRAINT [FK_system_config_system_config] FOREIGN KEY([id])
REFERENCES [dbo].[system_config] ([id])
GO
ALTER TABLE [dbo].[system_config] CHECK CONSTRAINT [FK_system_config_system_config]
GO
ALTER TABLE [dbo].[user_profile]  WITH CHECK ADD  CONSTRAINT [FK_user_profile_user_profile] FOREIGN KEY([userID])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[user_profile] CHECK CONSTRAINT [FK_user_profile_user_profile]
GO
ALTER TABLE [dbo].[users]  WITH CHECK ADD  CONSTRAINT [FK_users_user_roles] FOREIGN KEY([roleID])
REFERENCES [dbo].[user_roles] ([id])
GO
ALTER TABLE [dbo].[users] CHECK CONSTRAINT [FK_users_user_roles]
GO
ALTER TABLE [dbo].[notereviews]  WITH CHECK ADD  CONSTRAINT [CK_notereviews] CHECK  (([rating]>(0) AND [rating]<(6)))
GO
ALTER TABLE [dbo].[notereviews] CHECK CONSTRAINT [CK_notereviews]
GO
USE [master]
GO
ALTER DATABASE [notemarketplace] SET  READ_WRITE 
GO
