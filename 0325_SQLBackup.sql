USE [master]
GO
/****** Object:  Database [FormSystemDB]    Script Date: 2022/3/25 下午 07:27:25 ******/
IF exists (select * from sysdatabases where name='FormSystemDB')
		drop database FormSystemDB
GO

DECLARE @device_directory NVARCHAR(520)
SELECT @device_directory = SUBSTRING(filename, 1, CHARINDEX(N'master.mdf', LOWER(filename)) - 1)
FROM master.dbo.sysaltfiles WHERE dbid = 1 AND fileid = 1

EXECUTE (N'CREATE DATABASE FormSystemDB
  ON PRIMARY (NAME = N''FormSystemDB'', FILENAME = N''' + @device_directory + N'FormSystemDB.mdf'')
  LOG ON (NAME = N''FormSystemDB_log'',  FILENAME = N''' + @device_directory + N'FormSystemDB_log.ldf'')')
GO

ALTER DATABASE [FormSystemDB] SET COMPATIBILITY_LEVEL = 150
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [FormSystemDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [FormSystemDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [FormSystemDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [FormSystemDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [FormSystemDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [FormSystemDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [FormSystemDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [FormSystemDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [FormSystemDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [FormSystemDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [FormSystemDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [FormSystemDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [FormSystemDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [FormSystemDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [FormSystemDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [FormSystemDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [FormSystemDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [FormSystemDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [FormSystemDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [FormSystemDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [FormSystemDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [FormSystemDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [FormSystemDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [FormSystemDB] SET RECOVERY FULL 
GO
ALTER DATABASE [FormSystemDB] SET  MULTI_USER 
GO
ALTER DATABASE [FormSystemDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [FormSystemDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [FormSystemDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [FormSystemDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [FormSystemDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [FormSystemDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'FormSystemDB', N'ON'
GO
ALTER DATABASE [FormSystemDB] SET QUERY_STORE = OFF
GO
USE [FormSystemDB]
GO
/****** Object:  Table [dbo].[FormData]    Script Date: 2022/3/25 下午 07:27:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FormData](
	[DataID] [int] IDENTITY(1,1) NOT NULL,
	[FormID] [uniqueidentifier] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[AnswerData] [nvarchar](max) NULL,
	[QuestionSort] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_FormData] PRIMARY KEY CLUSTERED 
(
	[DataID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FormInfo]    Script Date: 2022/3/25 下午 07:27:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FormInfo](
	[FormID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Body] [nvarchar](max) NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_FormInfo] PRIMARY KEY CLUSTERED 
(
	[FormID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FormLayout]    Script Date: 2022/3/25 下午 07:27:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FormLayout](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FormID] [uniqueidentifier] NOT NULL,
	[QuestionType] [nvarchar](10) NOT NULL,
	[Body] [nvarchar](50) NOT NULL,
	[Answer] [nvarchar](50) NULL,
	[NeedAns] [bit] NOT NULL,
	[QuestionSort] [int] NOT NULL,
 CONSTRAINT [PK_FormLayout] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FrenquenQuestion]    Script Date: 2022/3/25 下午 07:27:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FrenquenQuestion](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Body] [nvarchar](50) NOT NULL,
	[Answer] [nvarchar](50) NULL,
	[QuestionType] [nvarchar](10) NOT NULL,
	[NeedAns] [bit] NOT NULL,
 CONSTRAINT [PK_FrenquenQuestion] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QuestionType]    Script Date: 2022/3/25 下午 07:27:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuestionType](
	[TypeID] [nvarchar](10) NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_QuestionType] PRIMARY KEY CLUSTERED 
(
	[TypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[FormData] ON 
GO
INSERT [dbo].[FormData] ([DataID], [FormID], [CreateDate], [AnswerData], [QuestionSort]) VALUES (1, N'e9fb425e-b4eb-497f-8b20-e2bc38eebc4d', CAST(N'2021-10-28T19:40:42.000' AS DateTime), N'ㄅ;1234567890;23123@email;123;+7/77(金甄女中);false,這是答案2,Answer3,false;', N'QT01;QT02;QT03;QT02;QT05;QT06;')
GO
INSERT [dbo].[FormData] ([DataID], [FormID], [CreateDate], [AnswerData], [QuestionSort]) VALUES (4, N'76fd1aff-3b06-43aa-a968-1f37f0e630e6', CAST(N'2021-11-27T00:00:00.000' AS DateTime), N'123;', N'QT02 ;')
GO
INSERT [dbo].[FormData] ([DataID], [FormID], [CreateDate], [AnswerData], [QuestionSort]) VALUES (1004, N'76fd1aff-3b06-43aa-a968-1f37f0e630e6', CAST(N'2022-02-12T00:00:00.000' AS DateTime), N'123123;', N'QT02 ;')
GO
INSERT [dbo].[FormData] ([DataID], [FormID], [CreateDate], [AnswerData], [QuestionSort]) VALUES (1005, N'e9fb425e-b4eb-497f-8b20-e2bc38eebc4d', CAST(N'2022-03-09T04:12:04.000' AS DateTime), N'123123;6;23123@email;10;+7/77(金甄女中);false,這是答案2,false,巴拉巴拉;', N'QT01;QT02;QT03;QT02;QT05;QT06;')
GO
INSERT [dbo].[FormData] ([DataID], [FormID], [CreateDate], [AnswerData], [QuestionSort]) VALUES (1011, N'e9fb425e-b4eb-497f-8b20-e2bc38eebc4d', CAST(N'2022-03-10T07:02:14.000' AS DateTime), N'Noah Travis;0958877663;magnis.dis.parturient@aol.com;48;wen wen(金門高中);false,false,Answer3,巴拉巴拉;', N'QT01;QT02;QT03;QT02;QT05;QT06;')
GO
INSERT [dbo].[FormData] ([DataID], [FormID], [CreateDate], [AnswerData], [QuestionSort]) VALUES (1012, N'e9fb425e-b4eb-497f-8b20-e2bc38eebc4d', CAST(N'2022-03-10T10:29:12.000' AS DateTime), N'Jeanette Ferrell;0953637564;netus.et@outlook.edu;15;核廢料(建國中學);答案1,false,false,巴拉巴拉;', N'QT01;QT02;QT03;QT02;QT05;QT06;')
GO
INSERT [dbo].[FormData] ([DataID], [FormID], [CreateDate], [AnswerData], [QuestionSort]) VALUES (1013, N'e9fb425e-b4eb-497f-8b20-e2bc38eebc4d', CAST(N'2022-03-10T11:06:23.000' AS DateTime), N'Marsden Mack;0987713372;vestibulum.massa@aol.ca;47;+7/77(金甄女中);答案1,false,Answer3,false;', N'QT01;QT02;QT03;QT02;QT05;QT06;')
GO
INSERT [dbo].[FormData] ([DataID], [FormID], [CreateDate], [AnswerData], [QuestionSort]) VALUES (1014, N'e9fb425e-b4eb-497f-8b20-e2bc38eebc4d', CAST(N'2022-03-10T14:23:56.000' AS DateTime), N'Nicholas Dennis;0978257733;vivamus.euismod.urna@yahoo.org;48;wen wen(金門高中);false,這是答案2,false,false;', N'QT01;QT02;QT03;QT02;QT05;QT06;')
GO
INSERT [dbo].[FormData] ([DataID], [FormID], [CreateDate], [AnswerData], [QuestionSort]) VALUES (1015, N'e9fb425e-b4eb-497f-8b20-e2bc38eebc4d', CAST(N'2022-03-10T16:00:39.000' AS DateTime), N'Quail Peck;0996324347;orci.lobortis@protonmail.couk;38;uliy(基隆高中);false,這是答案2,false,false;', N'QT01;QT02;QT03;QT02;QT05;QT06;')
GO
INSERT [dbo].[FormData] ([DataID], [FormID], [CreateDate], [AnswerData], [QuestionSort]) VALUES (1016, N'e9fb425e-b4eb-497f-8b20-e2bc38eebc4d', CAST(N'2022-03-10T17:00:24.000' AS DateTime), N'Stone Hood;0934283744;nulla.semper@icloud.com;23;核廢料(建國中學);答案1,false,false,false;', N'QT01;QT02;QT03;QT02;QT05;QT06;')
GO
INSERT [dbo].[FormData] ([DataID], [FormID], [CreateDate], [AnswerData], [QuestionSort]) VALUES (1017, N'e9fb425e-b4eb-497f-8b20-e2bc38eebc4d', CAST(N'2022-03-10T17:01:00.000' AS DateTime), N'Erich Walker;0986537625;mus.donec.dignissim@google.com;32;uliy(基隆高中);false,false,Answer3,巴拉巴拉;', N'QT01;QT02;QT03;QT02;QT05;QT06;')
GO
INSERT [dbo].[FormData] ([DataID], [FormID], [CreateDate], [AnswerData], [QuestionSort]) VALUES (1018, N'e9fb425e-b4eb-497f-8b20-e2bc38eebc4d', CAST(N'2022-03-10T19:00:15.000' AS DateTime), N'Tucker Leblanc;0963384382;ad.litora@google.net;22;核廢料(建國中學);false,這是答案2,false,false;', N'QT01;QT02;QT03;QT02;QT05;QT06;')
GO
INSERT [dbo].[FormData] ([DataID], [FormID], [CreateDate], [AnswerData], [QuestionSort]) VALUES (1019, N'e9fb425e-b4eb-497f-8b20-e2bc38eebc4d', CAST(N'2022-03-11T10:11:01.637' AS DateTime), N'Lacy Mccray;0968455569;euismod.in@icloud.ca;33;核廢料(建國中學);答案1,這是答案2,Answer3,巴拉巴拉;', N'QT01;QT02;QT03;QT02;QT05;QT06;')
GO
INSERT [dbo].[FormData] ([DataID], [FormID], [CreateDate], [AnswerData], [QuestionSort]) VALUES (1020, N'e9fb425e-b4eb-497f-8b20-e2bc38eebc4d', CAST(N'2022-03-11T10:11:46.483' AS DateTime), N'Iona England;0918823258;malesuada.malesuada@aol.org;57;核廢料(建國中學);答案1,false,false,巴拉巴拉;', N'QT01;QT02;QT03;QT02;QT05;QT06;')
GO
SET IDENTITY_INSERT [dbo].[FormData] OFF
GO
INSERT [dbo].[FormInfo] ([FormID], [Name], [Body], [StartDate], [EndDate], [CreateDate]) VALUES (N'76fd1aff-3b06-43aa-a968-1f37f0e630e6', N'UpdateForm', N'123456abc', CAST(N'2021-10-01T00:00:00.000' AS DateTime), CAST(N'2021-11-30T00:00:00.000' AS DateTime), CAST(N'2021-10-30T23:06:54.597' AS DateTime))
GO
INSERT [dbo].[FormInfo] ([FormID], [Name], [Body], [StartDate], [EndDate], [CreateDate]) VALUES (N'9ec616d3-5bb6-4483-90af-450d2f6af713', N'測試多項問題', N'ddddddddd', CAST(N'2021-10-01T00:00:00.000' AS DateTime), CAST(N'2021-10-31T00:00:00.000' AS DateTime), CAST(N'2021-10-26T18:59:06.130' AS DateTime))
GO
INSERT [dbo].[FormInfo] ([FormID], [Name], [Body], [StartDate], [EndDate], [CreateDate]) VALUES (N'84f66add-ccb9-4160-89a6-5477f79b6050', N'常用22222', N'jyhgjfghj', CAST(N'2022-03-09T00:00:00.000' AS DateTime), CAST(N'2022-03-31T00:00:00.000' AS DateTime), CAST(N'2022-03-22T21:33:10.917' AS DateTime))
GO
INSERT [dbo].[FormInfo] ([FormID], [Name], [Body], [StartDate], [EndDate], [CreateDate]) VALUES (N'f76e1202-77a8-452f-bdc7-711e1f7ea070', N'常用22222重構123', N'常用22222重構', CAST(N'2022-03-09T00:00:00.000' AS DateTime), CAST(N'2022-03-31T00:00:00.000' AS DateTime), CAST(N'2022-03-09T09:55:41.727' AS DateTime))
GO
INSERT [dbo].[FormInfo] ([FormID], [Name], [Body], [StartDate], [EndDate], [CreateDate]) VALUES (N'fe225a7f-2958-408a-99d0-911b77561a7a', N'testForm', N'Kjhfds;lfshdf', CAST(N'2021-10-25T00:00:00.000' AS DateTime), CAST(N'2021-10-27T00:00:00.000' AS DateTime), CAST(N'2021-10-26T17:50:51.060' AS DateTime))
GO
INSERT [dbo].[FormInfo] ([FormID], [Name], [Body], [StartDate], [EndDate], [CreateDate]) VALUES (N'4d6cd1b5-0a4a-4d40-951f-9684270f20bd', N'111', N'2', CAST(N'2022-03-04T00:00:00.000' AS DateTime), CAST(N'2022-03-25T00:00:00.000' AS DateTime), CAST(N'2022-03-04T21:12:36.320' AS DateTime))
GO
INSERT [dbo].[FormInfo] ([FormID], [Name], [Body], [StartDate], [EndDate], [CreateDate]) VALUES (N'defc0a01-888b-4132-9578-c417fffc9d34', N'testDelete', N'ytrutyutr', CAST(N'2022-04-01T00:00:00.000' AS DateTime), CAST(N'2022-04-30T00:00:00.000' AS DateTime), CAST(N'2022-03-22T21:33:47.263' AS DateTime))
GO
INSERT [dbo].[FormInfo] ([FormID], [Name], [Body], [StartDate], [EndDate], [CreateDate]) VALUES (N'3ad3e869-6dd8-41d7-845f-c92ac8a4c8ba', N'ㄇ1', N'asdazxczgdfg', CAST(N'2021-12-22T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2021-12-31T10:35:34.810' AS DateTime))
GO
INSERT [dbo].[FormInfo] ([FormID], [Name], [Body], [StartDate], [EndDate], [CreateDate]) VALUES (N'e9fb425e-b4eb-497f-8b20-e2bc38eebc4d', N'測試用表單', N'測試用的表單，可隨意編輯', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-12-31T00:00:00.000' AS DateTime), CAST(N'2021-10-21T00:00:00.000' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[FormLayout] ON 
GO
INSERT [dbo].[FormLayout] ([ID], [FormID], [QuestionType], [Body], [Answer], [NeedAns], [QuestionSort]) VALUES (1, N'e9fb425e-b4eb-497f-8b20-e2bc38eebc4d', N'QT01 ', N'姓名', NULL, 1, 1)
GO
INSERT [dbo].[FormLayout] ([ID], [FormID], [QuestionType], [Body], [Answer], [NeedAns], [QuestionSort]) VALUES (3, N'e9fb425e-b4eb-497f-8b20-e2bc38eebc4d', N'QT02 ', N'手機', NULL, 1, 2)
GO
INSERT [dbo].[FormLayout] ([ID], [FormID], [QuestionType], [Body], [Answer], [NeedAns], [QuestionSort]) VALUES (5, N'e9fb425e-b4eb-497f-8b20-e2bc38eebc4d', N'QT03 ', N'Email', NULL, 1, 3)
GO
INSERT [dbo].[FormLayout] ([ID], [FormID], [QuestionType], [Body], [Answer], [NeedAns], [QuestionSort]) VALUES (6, N'e9fb425e-b4eb-497f-8b20-e2bc38eebc4d', N'QT02 ', N'年齡', NULL, 1, 4)
GO
INSERT [dbo].[FormLayout] ([ID], [FormID], [QuestionType], [Body], [Answer], [NeedAns], [QuestionSort]) VALUES (8, N'e9fb425e-b4eb-497f-8b20-e2bc38eebc4d', N'QT05 ', N'1. 請投票給下面一位', N'核廢料(建國中學);+7/77(金甄女中);wen wen(金門高中);uliy(基隆高中)', 1, 5)
GO
INSERT [dbo].[FormLayout] ([ID], [FormID], [QuestionType], [Body], [Answer], [NeedAns], [QuestionSort]) VALUES (10, N'e9fb425e-b4eb-497f-8b20-e2bc38eebc4d', N'QT06 ', N'2. 測試複選', N'答案1;這是答案2;Answer3;巴拉巴拉', 0, 6)
GO
INSERT [dbo].[FormLayout] ([ID], [FormID], [QuestionType], [Body], [Answer], [NeedAns], [QuestionSort]) VALUES (14, N'9ec616d3-5bb6-4483-90af-450d2f6af713', N'QT05 ', N'1.單選', N'1;2;3;4', 0, 0)
GO
INSERT [dbo].[FormLayout] ([ID], [FormID], [QuestionType], [Body], [Answer], [NeedAns], [QuestionSort]) VALUES (15, N'9ec616d3-5bb6-4483-90af-450d2f6af713', N'QT02 ', N'2.文字問題(數字)', NULL, 0, 1)
GO
INSERT [dbo].[FormLayout] ([ID], [FormID], [QuestionType], [Body], [Answer], [NeedAns], [QuestionSort]) VALUES (16, N'9ec616d3-5bb6-4483-90af-450d2f6af713', N'QT04 ', N'3.問題u', NULL, 0, 2)
GO
INSERT [dbo].[FormLayout] ([ID], [FormID], [QuestionType], [Body], [Answer], [NeedAns], [QuestionSort]) VALUES (32, N'3ad3e869-6dd8-41d7-845f-c92ac8a4c8ba', N'QT05 ', N'1.單選', N'1;2;3;4', 0, 1)
GO
INSERT [dbo].[FormLayout] ([ID], [FormID], [QuestionType], [Body], [Answer], [NeedAns], [QuestionSort]) VALUES (33, N'3ad3e869-6dd8-41d7-845f-c92ac8a4c8ba', N'QT04 ', N'eeeee', NULL, 1, 2)
GO
INSERT [dbo].[FormLayout] ([ID], [FormID], [QuestionType], [Body], [Answer], [NeedAns], [QuestionSort]) VALUES (1034, N'76fd1aff-3b06-43aa-a968-1f37f0e630e6', N'QT05 ', N'1.單選', N'1;2;3;4', 1, 2)
GO
INSERT [dbo].[FormLayout] ([ID], [FormID], [QuestionType], [Body], [Answer], [NeedAns], [QuestionSort]) VALUES (1035, N'76fd1aff-3b06-43aa-a968-1f37f0e630e6', N'QT06 ', N'測試複選方塊', N'選項1;選項2;選項3;選項4', 1, 1)
GO
INSERT [dbo].[FormLayout] ([ID], [FormID], [QuestionType], [Body], [Answer], [NeedAns], [QuestionSort]) VALUES (1043, N'f76e1202-77a8-452f-bdc7-711e1f7ea070', N'QT02 ', N'12', NULL, 1, 2)
GO
INSERT [dbo].[FormLayout] ([ID], [FormID], [QuestionType], [Body], [Answer], [NeedAns], [QuestionSort]) VALUES (1044, N'f76e1202-77a8-452f-bdc7-711e1f7ea070', N'QT05 ', N'1.單選', N'1;2;3;4', 1, 5)
GO
INSERT [dbo].[FormLayout] ([ID], [FormID], [QuestionType], [Body], [Answer], [NeedAns], [QuestionSort]) VALUES (1045, N'f76e1202-77a8-452f-bdc7-711e1f7ea070', N'QT06 ', N'測試複選方塊', N'選項1;選項2;選項3;選項4', 1, 6)
GO
INSERT [dbo].[FormLayout] ([ID], [FormID], [QuestionType], [Body], [Answer], [NeedAns], [QuestionSort]) VALUES (1046, N'f76e1202-77a8-452f-bdc7-711e1f7ea070', N'QT04 ', N'文字方塊(日期)', NULL, 0, 4)
GO
INSERT [dbo].[FormLayout] ([ID], [FormID], [QuestionType], [Body], [Answer], [NeedAns], [QuestionSort]) VALUES (1047, N'f76e1202-77a8-452f-bdc7-711e1f7ea070', N'QT04 ', N'689', N'1;2;3;4', 0, 1)
GO
INSERT [dbo].[FormLayout] ([ID], [FormID], [QuestionType], [Body], [Answer], [NeedAns], [QuestionSort]) VALUES (1048, N'f76e1202-77a8-452f-bdc7-711e1f7ea070', N'QT01 ', N'測試文字方塊', NULL, 0, 3)
GO
INSERT [dbo].[FormLayout] ([ID], [FormID], [QuestionType], [Body], [Answer], [NeedAns], [QuestionSort]) VALUES (1049, N'84f66add-ccb9-4160-89a6-5477f79b6050', N'QT01 ', N'測試文字方塊', NULL, 0, 1)
GO
INSERT [dbo].[FormLayout] ([ID], [FormID], [QuestionType], [Body], [Answer], [NeedAns], [QuestionSort]) VALUES (1050, N'84f66add-ccb9-4160-89a6-5477f79b6050', N'QT03 ', N'Email333:', NULL, 1, 2)
GO
INSERT [dbo].[FormLayout] ([ID], [FormID], [QuestionType], [Body], [Answer], [NeedAns], [QuestionSort]) VALUES (1051, N'84f66add-ccb9-4160-89a6-5477f79b6050', N'QT03 ', N'Email:', NULL, 0, 3)
GO
INSERT [dbo].[FormLayout] ([ID], [FormID], [QuestionType], [Body], [Answer], [NeedAns], [QuestionSort]) VALUES (1052, N'defc0a01-888b-4132-9578-c417fffc9d34', N'QT06 ', N'asdasdss123', N'5;6;7;8', 1, 1)
GO
INSERT [dbo].[FormLayout] ([ID], [FormID], [QuestionType], [Body], [Answer], [NeedAns], [QuestionSort]) VALUES (1053, N'defc0a01-888b-4132-9578-c417fffc9d34', N'QT01 ', N'名稱:', NULL, 1, 2)
GO
INSERT [dbo].[FormLayout] ([ID], [FormID], [QuestionType], [Body], [Answer], [NeedAns], [QuestionSort]) VALUES (1054, N'defc0a01-888b-4132-9578-c417fffc9d34', N'QT02 ', N'電話:', NULL, 1, 3)
GO
SET IDENTITY_INSERT [dbo].[FormLayout] OFF
GO
SET IDENTITY_INSERT [dbo].[FrenquenQuestion] ON 
GO
INSERT [dbo].[FrenquenQuestion] ([ID], [Name], [Body], [Answer], [QuestionType], [NeedAns]) VALUES (1, N'常用問題1', N'測試文字方塊', NULL, N'QT01 ', 0)
GO
INSERT [dbo].[FrenquenQuestion] ([ID], [Name], [Body], [Answer], [QuestionType], [NeedAns]) VALUES (2, N'常用問題2', N'測試複選方塊', N'選項1;選項2;選項3;選項4', N'QT06 ', 1)
GO
INSERT [dbo].[FrenquenQuestion] ([ID], [Name], [Body], [Answer], [QuestionType], [NeedAns]) VALUES (3, N'測試特殊名稱', N'文字方塊(日期)', NULL, N'QT04 ', 0)
GO
INSERT [dbo].[FrenquenQuestion] ([ID], [Name], [Body], [Answer], [QuestionType], [NeedAns]) VALUES (22, N'常用問題3', N'1.單選', N'1;2;3;4', N'QT05 ', 1)
GO
INSERT [dbo].[FrenquenQuestion] ([ID], [Name], [Body], [Answer], [QuestionType], [NeedAns]) VALUES (1026, N'常用問題123', N'3.單選', N'1;2;3;4', N'QT05 ', 1)
GO
INSERT [dbo].[FrenquenQuestion] ([ID], [Name], [Body], [Answer], [QuestionType], [NeedAns]) VALUES (1027, N'常用問題4', N'689', N'1;2;3;4', N'QT04 ', 0)
GO
INSERT [dbo].[FrenquenQuestion] ([ID], [Name], [Body], [Answer], [QuestionType], [NeedAns]) VALUES (1028, N'名稱', N'名稱:', NULL, N'QT01 ', 0)
GO
INSERT [dbo].[FrenquenQuestion] ([ID], [Name], [Body], [Answer], [QuestionType], [NeedAns]) VALUES (1029, N'電話', N'電話:', NULL, N'QT02 ', 1)
GO
INSERT [dbo].[FrenquenQuestion] ([ID], [Name], [Body], [Answer], [QuestionType], [NeedAns]) VALUES (1030, N'Email', N'Email:', NULL, N'QT03 ', 0)
GO
INSERT [dbo].[FrenquenQuestion] ([ID], [Name], [Body], [Answer], [QuestionType], [NeedAns]) VALUES (1031, N'複選', N'asdasdss123', N'5;6;7;8', N'QT06 ', 0)
GO
SET IDENTITY_INSERT [dbo].[FrenquenQuestion] OFF
GO
INSERT [dbo].[QuestionType] ([TypeID], [Name]) VALUES (N'QT01 ', N'文字方塊(一般)            ')
GO
INSERT [dbo].[QuestionType] ([TypeID], [Name]) VALUES (N'QT02 ', N'文字方塊(數字)            ')
GO
INSERT [dbo].[QuestionType] ([TypeID], [Name]) VALUES (N'QT03 ', N'文字方塊(Email)         ')
GO
INSERT [dbo].[QuestionType] ([TypeID], [Name]) VALUES (N'QT04 ', N'文字方塊(日期)            ')
GO
INSERT [dbo].[QuestionType] ([TypeID], [Name]) VALUES (N'QT05 ', N'單選方塊                ')
GO
INSERT [dbo].[QuestionType] ([TypeID], [Name]) VALUES (N'QT06 ', N'複選方塊                ')
GO
ALTER TABLE [dbo].[FormData]  WITH CHECK ADD  CONSTRAINT [FK_FormData_FormInfo] FOREIGN KEY([FormID])
REFERENCES [dbo].[FormInfo] ([FormID])
GO
ALTER TABLE [dbo].[FormData] CHECK CONSTRAINT [FK_FormData_FormInfo]
GO
ALTER TABLE [dbo].[FormLayout]  WITH CHECK ADD  CONSTRAINT [FK_FormLayout_FormInfo] FOREIGN KEY([FormID])
REFERENCES [dbo].[FormInfo] ([FormID])
GO
ALTER TABLE [dbo].[FormLayout] CHECK CONSTRAINT [FK_FormLayout_FormInfo]
GO
ALTER TABLE [dbo].[FormLayout]  WITH CHECK ADD  CONSTRAINT [FK_FormLayout_QuestionType] FOREIGN KEY([QuestionType])
REFERENCES [dbo].[QuestionType] ([TypeID])
GO
ALTER TABLE [dbo].[FormLayout] CHECK CONSTRAINT [FK_FormLayout_QuestionType]
GO
ALTER TABLE [dbo].[FrenquenQuestion]  WITH CHECK ADD  CONSTRAINT [FK_FrenquenQuestion_QuestionType] FOREIGN KEY([QuestionType])
REFERENCES [dbo].[QuestionType] ([TypeID])
GO
ALTER TABLE [dbo].[FrenquenQuestion] CHECK CONSTRAINT [FK_FrenquenQuestion_QuestionType]
GO
USE [master]
GO
ALTER DATABASE [FormSystemDB] SET  READ_WRITE 
GO
