
/****** Object:  StoredProcedure [dbo].[uspGetDlDownloadDetails]    Script Date: 11/26/2015 16:38:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ============================================================    
-- Author:  Cognizant Tech Solutions    
-- Create date: 13/10/2011     
-- Description: To get the DL package details
-- ============================================================    
ALTER PROCEDURE [dbo].[uspGetDlDownloadDetails]    
 @DlId INT
AS    
BEGIN 
	BEGIN TRY   
	
 DECLARE @countrycode NVARCHAR(20)
	 SELECT @countrycode=DLDetailsXML.query('/dpsi').value('(/dpsi/@countrycode)[1]', 'nvarchar(20)') FROM dbo.tblDLMaster
	  WHERE DLID=@DlId
		   
		   
		SELECT DLID,InitVectorKey,ContentEncryptionKey,DestinationPath,@countrycode AS CountryCode FROM dbo.tblLooseleaf
		WHERE DLID = @DlId AND IsActive = 1;
		
		
  
 	END TRY
	BEGIN CATCH
	BEGIN
		DECLARE @ErrorMessage NVARCHAR(4000);    
		DECLARE @ErrorSeverity INT;    
		DECLARE @ErrorState INT;    

	    SELECT @ErrorMessage = ERROR_MESSAGE(),    
			   @ErrorSeverity = ERROR_SEVERITY(),    
			   @ErrorState = ERROR_STATE();    
	   RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);        
			  
	END 
	END CATCH
END


GO


