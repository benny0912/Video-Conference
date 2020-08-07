USE [telemeetDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[user_email] [varchar] (50) NOT NULL,
	[user_password] [varchar] (100) NOT NULL,
	[user_first_name] [varchar] (50),
	[user_last_name] [varchar] (50),
	[date_created] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP,
	[signed_in] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP,
	[last_room] [varchar] (50),
	[user_role] [varchar] (10) NOT NULL,
	[user_image] [nvarchar] (MAX),
	[image_created] [datetime],

 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[user_email]
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ActivityLogs](
	[activity_log_id] [int] IDENTITY NOT NULL,
	[user_email] [varchar] (50) NOT NULL,
	[user_first_name] [varchar] (50),
	[user_last_name] [varchar] (50),
	[activity_type] [varchar] (50) NULL,
	[activity_details] [varchar] (50) NULL,
	[last_activity] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY (activity_log_id),
 CONSTRAINT [FK_ActivityLogs] FOREIGN KEY  ([user_email]) 
 REFERENCES Users(user_email)
)
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Rooms](
	[room_id] [varchar] (50) NOT NULL,
	[room_name] [varchar] (50) NOT NULL,
	[room_password] [varchar] (MAX),
	[user_email] [varchar] (50),	
	PRIMARY KEY (room_id),
 CONSTRAINT [FK_rooms] FOREIGN KEY  ([user_email]) 
 REFERENCES Users(user_email)
)
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoomsUsers](
	[room_id] [varchar] (50) NOT NULL,
	[user_email] [varchar] (50),	
 CONSTRAINT [FK_roomsUsers] FOREIGN KEY  ([room_id]) 
 REFERENCES Rooms(room_id)
 ON DELETE CASCADE
)
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CameraTime](
	[time_id] [nchar] (10) NOT NULL,
	[capture_time] [bigint],
	PRIMARY KEY (time_id)
	)
GO
