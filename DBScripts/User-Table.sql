-- Script Date: 4/1/2015 10:41  - ErikEJ.SqlCeScripting version 3.5.2.49
DROP TABLE [User];
CREATE TABLE [User] (
  [UserId] integer NOT NULL PRIMARY KEY AUTOINCREMENT, 
  [LastSuccessfulLogin] datetime, 
  [LockedStatus] bool, 
  [Email] [nvarchar(50)], 
  [Password] [NVARCHAR(50)], 
  [SymmetricKey] [nvarchar(50)], 
  [FirstName] [nvarchar(100)], 
  [LastName] [nvarchar(50)], 
  [CountryCode] [nvarchar(10)] NOT NULL, 
  [SyncAnnotations] BOOL, 
  [LastSyncDate] DATETIME, 
  [IsAlive] BOOL, 
  [NeedChangePassword] BOOL);
  
  DROP TABLE [DLBook];
  CREATE TABLE [DLBook] (
  [RowId] INTEGER NOT NULL PRIMARY KEY, 
  [Email] [VARCHAR(50)], 
  [CountryCode] [VARCHAR(5)],
  [BookId] [VARCHAR(50)], 
  [IsDeleted] BOOL DEFAULT 0, 
  [Name] [VARCHAR(100)], 
  [Author] VARCHAR,
  [Description] [VARCHAR(500)], 
  [CurrentVersion] [VARCHAR(50)], 
  [Size] [VARCHAR(50)], 
  [LastUpdatedDate] DATETIME, 
  [LastDownloadedVersion] [VARCHAR(50)], 
  [DpsiCode] VARCHAR, 
  [FileUrl] [VARCHAR(100)], 
  [InitVector] VARCHAR, 
  [K2Key] VARCHAR, 
  [HmacKey] VARCHAR, 
  [ColorPrimary] [varchar2(7)] NOT NULL DEFAULT '#7D7D7D', 
  [ColorSecondary] [varchar2(7)] NOT NULL DEFAULT '#7D7D7D', 
  [FontColor] [varchar2(7)] NOT NULL DEFAULT '#7D7D7D', 
  [IsLoan] BOOL DEFAULT 0, 
  [ValidTo] DATETIME, 
  [DownloadStatus] BOOL, 
  [OrderBy] INTEGER
 );

