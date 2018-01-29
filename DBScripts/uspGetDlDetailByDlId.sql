USE [LN_DL_SubMgmt_P2]
GO

/****** Object:  StoredProcedure [dbo].[uspGetDlDetailByDlId]    Script Date: 06/12/2015 09:03:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





-- =============================================      
-- Author     : allen      
-- Create date: 20150601    
-- Description: This stored procedure is used to       
--    get the device and DL details for      
--    the given dlid.
-- =============================================      
ALTER PROCEDURE [dbo].[uspGetDlDetailByDlId]       
 @Username varchar(50),      
 @DeviceId varchar(500),
 @DlId INT      
AS      
BEGIN      
	SET NOCOUNT ON;      
    BEGIN TRY      
		
		DECLARE @DeleteByEreaderStatusCode INT = 0,@DeletebyUserStatusCode INT = 0, @UserID INT = 0,
		@NotDownloadedStatusCode INT =0;
		 
		SELECT @NotDownloadedStatusCode = Id FROM dbo.tblDLStatus WHERE [Description] ='Not downloaded' AND IsActive = 1; 
		SELECT @DeletebyUserStatusCode = Id FROM tblDLStatus WHERE [Description] ='Removed by user' AND IsActive = 1;
		SELECT @DeleteByEreaderStatusCode = Id FROM tblDLStatus WHERE [Description] ='Removed by eReader' AND IsActive =1;
		SELECT @UserID = UserID FROM dbo.tblUser WHERE EmailAddress = @Username AND IsActive = 1;
		
		INSERT INTO [dbo].[tblDLToDeviceMappingHst]
			([DLDeviceMappingId]
			,[DeviceId]
			,[DLID]
			,[UserID]
			,[DLVersion]
			,[DLStatus]
			,[LastUpdatedDate]
			,[LastUpdatedBy]
			,[CreatedDate]
			,[CreatedBy])
		SELECT [DLDeviceMappingId]
			   ,[DeviceId]
			   ,[DLID]
			   ,[UserID]
			   ,[DLVersion]
			   ,[DLStatus]
			   ,[LastUpdatedDate]
			   ,[LastUpdatedBy]
			   ,[CreatedDate]
			   ,[CreatedBy]
		FROM [dbo].[tblDLToDeviceMapping]
		WHERE [UserID] = @UserID
				AND DLID IN (SELECT ent.[DLID] 
							FROM [dbo].[tblEntitlements] ent
									INNER JOIN
								[dbo].[tblDLToDeviceMapping] dtdm
									ON dtdm.DLID = ent.DLID AND dtdm.[UserID] = ent.[UserID]
							WHERE ent.[IsActive] = 1 
								AND ent.[UserID] = @UserID
								AND CAST(ent.[ValidTo] AS DATE) < CAST(GETDATE() AS DATE)
								AND ent.[DLID] NOT IN (SELECT eent.[DLID] FROM [tblEntitlements] eent WHERE eent.IsActive=1 AND eent.UserID=@UserID AND CAST(eent.[ValidTo] AS DATE) >= CAST(GETDATE() AS DATE))
								AND ISNULL(dtdm.[DLStatus],0) <> @DeleteByEreaderStatusCode 
								AND ent.DLID=@DlId--added by allen 20150601
							GROUP BY ent.DLID)
		
		
		--ER 47
		UPDATE [dbo].[tblSyncDeviceInfo]
		SET [LSST] = '1-1-2000',
			LastModifiedOn = GETUTCDATE()
		WHERE [SyncID] IN (SELECT SyncID 
						   FROM tblSyncMaster WHERE 
							[DLID] IN (SELECT ent.[DLID] 
										FROM [dbo].[tblEntitlements] ent
											--INNER JOIN
											--[dbo].[tblDLToDeviceMapping] dtdm
											--ON dtdm.DLID = ent.DLID AND dtdm.[UserID] = ent.[UserID]
										WHERE ent.[IsActive] = 1 
											AND ent.[UserID] = @UserID
											AND CAST(ent.[ValidTo] AS DATE) < CAST(GETDATE() AS DATE)
											AND ent.[DLID] NOT IN (SELECT eent.[DLID] FROM [tblEntitlements] eent WHERE eent.IsActive=1 AND eent.UserID=@UserID AND CAST(eent.[ValidTo] AS DATE) >= CAST(GETDATE() AS DATE))
										     AND ent.DLID=@DlId--added by allen 20150601
											--AND ISNULL(dtdm.[DLStatus],0) <> @DeleteByEreaderStatusCode
										GROUP BY ent.DLID)
							AND [UserID] = @UserID) 
		AND [DeviceID] = @DeviceId
		
		UPDATE [dbo].[tblDLToDeviceMapping]
		SET
			DLStatus = @DeleteByEreaderStatusCode
		WHERE [UserID] = @UserID
				AND DLID IN (SELECT ent.[DLID] 
							FROM [dbo].[tblEntitlements] ent
									INNER JOIN
								[dbo].[tblDLToDeviceMapping] dtdm
									ON dtdm.DLID = ent.DLID AND dtdm.[UserID] = ent.[UserID]
							WHERE ent.[IsActive] = 1 
								AND ent.[UserID] = @UserID
								AND CAST(ent.[ValidTo] AS DATE) < CAST(GETDATE() AS DATE)
								AND ent.[DLID] NOT IN (SELECT eent.[DLID] FROM [tblEntitlements] eent WHERE eent.IsActive=1 AND eent.UserID=@UserID AND CAST(eent.[ValidTo] AS DATE) >= CAST(GETDATE() AS DATE))
								AND ISNULL(dtdm.[DLStatus],0) <> @DeleteByEreaderStatusCode
								AND ent.DLID=@DlId--added by allen 20150601
							GROUP BY ent.DLID)
		
		
		

		SELECT DISTINCT    
			[dl].[DLID] as [DLID], e.EntitlementId,     
			@DeviceId, 
			ISNULL([ddl].DLVersion,0) AS [LastDownloadedVersion],      
			[dl].[LatestVersion] AS [LatestVersion],      
			[dl].[Description] AS [Description],      
			[dl].[LastUpdatedDate] AS [LastUpdatedDate],      
			[dl].[FileSize] AS [FileSize],      
			[dl].[DLTitle] AS [DLTitle],
			e.IsLoan,
			e.ValidTo,
			e.Trial -- LNRED-153      
		FROM tblUser u    
			INNER JOIN [tblEntitlements] e ON e.UserID = u.UserID     
			INNER JOIN tblDL dl ON e.DLID = dl.DLID      
			LEFT OUTER JOIN tblDLToDeviceMapping ddl 
			ON ddl.DLID = dl.DLID AND ddl.UserID = u.UserID AND ddl.DeviceId = @DeviceId
		WHERE u.EmailAddress = @Username AND       
			u.IsActive=1  AND e.IsActive = 1 AND dl.IsActive = 1 
			AND DATEDIFF(D,GETDATE(),e.ValidTo) >= 0 AND DATEDIFF(D,GETDATE(),e.ValidFrom) <= 0 
			AND ISNULL(ddl.DLStatus,@NotDownloadedStatusCode) NOT IN (@DeletebyUserStatusCode,@DeleteByEreaderStatusCode)
			AND e.DLID=@DlId--added by allen 20150601
	END TRY      
	BEGIN CATCH      
		DECLARE @ErrorMessage NVARCHAR(4000);      
		DECLARE @ErrorSeverity INT;      
		DECLARE @ErrorState INT;      
      
		SELECT @ErrorMessage = ERROR_MESSAGE(),      
           @ErrorSeverity = ERROR_SEVERITY(),      
           @ErrorState = ERROR_STATE();      
		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);      
 END CATCH      
END






GO


