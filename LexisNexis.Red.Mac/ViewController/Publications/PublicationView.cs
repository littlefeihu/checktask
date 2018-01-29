
using System;
using System.Threading;

using Foundation;
using AppKit;
using ObjCRuntime;
using CoreGraphics;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Mac.Data;
using System.Threading.Tasks;

namespace LexisNexis.Red.Mac
{
	public partial class PublicationView : NSView
	{
		#region const
		const float PUBLICATION_VIEW_HEIGHT = 350f;

		const float PUBLICATION_COVER_ORGY = 80f;
		const float PUBLICATION_COVER_WIDTH = 200f;
		const float PUBLICATION_COVER_HEIGHT = 270f;
		const float PUBLICATION_COVER_HORIZONTAL_SPACING = 30f;

		const float PUBLICATION_COVERLINE_SPACE = 15f;

		const float PUBLICATION_TITLE_ORGY = 130+80f;
		const float PUBLICATION_LINT_WIDTH = 170f;
		const float PUBLICATION_LINT_HEIGHT = 110f;

		const float PUBLICATION_TITLE_LINTHEIGHT = 25f;

		const float PUBLICATION_LOAN_ORGY = 50+80f;
		const float PUBLICATION_LOANDAY_ORGY = 30+80f;

		const float PUBLICATION_DOWN_WIDTH = 90f;
		const float PUBLICATION_DOWN_ORGX = PUBLICATION_COVER_WIDTH-PUBLICATION_DOWN_WIDTH;
		const float PUBLICATION_DOWN_ORGY = PUBLICATION_VIEW_HEIGHT-PUBLICATION_DOWN_WIDTH;

		#endregion

		#region properties
		/// <summary>
		/// Gets or sets the index of publication view
		/// </summary>
		/// <value>index of publication view.</value>
		public int Index { get; set;}

		public Publication BookInfo { get; set;}

		//book cover sash
		TriangleView customTriView; 
		NSButton downloadButton;
		NSButton updateButton;
		NSButton expiredButton;
		NSTextField updateInfoLabel;
	
		bool downloadPreUpdate;
		CircularProgressView progressView;

		//
		NSTextField currencyDateTF;
		NSTextField titleStatusLabel;
		NSButton infoButton;
		bool isCanToContent;
		public int UpdateStatus{ get; set;}

		//
		bool isFTC;
		public DownloadManager DownloadController;
		public PublicationInfoPanelController infoController {get; set;}
		#endregion

		#region Constructors

		// Called when created from unmanaged code
		public PublicationView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public PublicationView (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		[Export("initWithFrame:")]
		public PublicationView (CGRect frame) : base(frame)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
			
		}
		#endregion

		#region methods
		public void InitializeValue (Publication book, int curIndex)
		{
			BookInfo = book;
			Index = curIndex;
			isFTC = BookInfo.IsFTC;
			UpdateStatus = 0;

			AddTitleFrame ();
			AddLoanFrame ();
			AddFTCImageView ();

			AddTitleStatusFrame ();
			AddInfoButtonFrame ();

			AddSashView ();
		
			NSNotificationCenter.DefaultCenter.AddObserver (LNRConstants.LNPublicationDidFinishedDownload, 
				HandleFinishDownload);

			DownloadController = new DownloadManager (this);
		}

		void AddFTCImageView ()
		{
			if (isFTC) {
				var frame = new CGRect (10, PUBLICATION_VIEW_HEIGHT - 50, 30, 50);
				var imageView = new NSImageView (frame);
				imageView.Image = Utility.ImageWithFilePath ("/Images/Publication/FTC.png");
				AddSubview (imageView);
			}
		}
			
		void AddTitleFrame ()
		{
			string titleName = BookInfo.Name;

			CGRect titleFrame;

			if (isFTC) {
				int location = titleName.IndexOf ("+ Cases");
				if (location > 0) {
					titleName = titleName.Substring (0, location);
				}else {
					location = titleName.IndexOf ("+Case");
					if (location > 0) {
						titleName = titleName.Substring (0, location);
					}
				}
				titleFrame = TitleCaseFrameWithTitle(titleName);
			} else {
				titleFrame = TitleFrameWithTitle(titleName);
			}



			var titleTF = new NSTextField (titleFrame);
			titleTF.Cell.Bordered = false;
			titleTF.Cell.DrawsBackground = false;
			titleTF.Cell.Editable = false;
			titleTF.Cell.Alignment = NSTextAlignment.Justified;
			titleTF.Cell.LineBreakMode = NSLineBreakMode.TruncatingTail;
			titleTF.Cell.TruncatesLastVisibleLine = true;
			titleTF.ToolTip = BookInfo.Name;

			NSAttributedString attributeTitle = Utility.AttributedTitle(titleName, 
				Utility.ColorWithHexColorValue (BookInfo.FontColor,1.0f), "Garamond",17.5f,NSTextAlignment.Center);
			titleTF.AttributedStringValue = attributeTitle;
			AddSubview (titleTF);
			//NSDictionary font = titleTF.AttributedStringValue.GetFontAttributes(new NSRange());
//
			if (isFTC) {
				CGRect caseFrame = new CGRect (titleFrame.Left,titleFrame.Top-2-PUBLICATION_TITLE_LINTHEIGHT,titleFrame.Width,PUBLICATION_TITLE_LINTHEIGHT);
				var caseTF = new NSTextField (caseFrame);
				caseTF.Cell.Bordered = false;
				caseTF.Cell.DrawsBackground = false;
				caseTF.Cell.Editable = false;
				caseTF.Cell.Alignment = NSTextAlignment.Justified;
				caseTF.Cell.LineBreakMode = NSLineBreakMode.TruncatingTail;
				caseTF.Cell.TruncatesLastVisibleLine = true;

				NSAttributedString caseTitle = Utility.AttributedTitle("+ Cases", 
					Utility.ColorWithHexColorValue (BookInfo.FontColor,1.0f), "Garamond",17.5f,NSTextAlignment.Center);
				caseTF.AttributedStringValue = caseTitle;
				AddSubview (caseTF);
			}
		}

		void AddLoanFrame ()
		{
			if (!BookInfo.IsLoan) {
				return;
			}
				
			var loanTF = new NSTextField (loanFrame);
			loanTF.Cell.Bordered = false;
			loanTF.Cell.DrawsBackground = false;
			loanTF.Cell.Editable = false;
			NSAttributedString attributeTitle = Utility.AttributedTitle("LOAN", NSColor.Grid, "Garamond",17.5f,NSTextAlignment.Center);
			loanTF.AttributedStringValue = attributeTitle;
			AddSubview (loanTF);

			string loanInfoStr = BookInfo.DaysRemaining + " days Remaining";
			var loanDateTF = new NSTextField (loanDateFrame);
			loanDateTF.Cell.Bordered = false;
			loanDateTF.Cell.DrawsBackground = false;
			loanDateTF.Cell.Editable = false;
			loanDateTF.Cell.LineBreakMode = NSLineBreakMode.TruncatingTail;
			loanDateTF.Cell.TruncatesLastVisibleLine = true;
			if (BookInfo.DaysRemaining > 0) {
				attributeTitle = Utility.AttributedTitle (loanInfoStr, NSColor.Grid, "Garamond", 17.5f, NSTextAlignment.Center);
				loanDateTF.AttributedStringValue = attributeTitle;
			} else {
				attributeTitle = Utility.AttributedTitle ("Due to expired", NSColor.Grid, "Garamond", 17.5f, NSTextAlignment.Center);
				loanDateTF.AttributedStringValue = attributeTitle;
			}
			AddSubview (loanDateTF);
		}

		void AddTitleStatusFrame ()
		{
			string statusInfoStr;
			string moreStatusInfoStr;
			switch (BookInfo.PublicationStatus) {
			case PublicationStatusEnum.Downloaded:
				statusInfoStr = "Up to date";
				moreStatusInfoStr = "Currency Date " + BookInfo.CurrencyDate.Value.ToString("dd MMM yyyy");
				break;

			case PublicationStatusEnum.NotDownloaded:
				statusInfoStr = "Download";
				double mbSize = ((double)BookInfo.Size) / 1024 / 1024;
				string mbSizeStr = mbSize.ToString ("0.00");
				int count = mbSizeStr.LastIndexOf (".");
				moreStatusInfoStr = mbSizeStr.Substring(0, count + 3)+ " MB";
				break;

			case PublicationStatusEnum.RequireUpdate:
				if (BookInfo.UpdateCount != 1) {
					statusInfoStr = BookInfo.UpdateCount + " Updates avaiable";
				} else {
					statusInfoStr = BookInfo.UpdateCount + " Update avaiable";
				}
				moreStatusInfoStr = "Currency Date " + BookInfo.CurrencyDate.Value.ToString("dd MMM yyyy");
				break;

			default:
				statusInfoStr = "";
				moreStatusInfoStr = "";
				break;
			}

			if (BookInfo.DaysRemaining < 0) {
				statusInfoStr = "Expired";
				//moreStatusInfoStr = "Currency Date " + BookInfo.CurrencyDate.Value.ToString("dd MMM yyyy");
			}

			if (titleStatusLabel == null) {
				titleStatusLabel = new NSTextField (titleStatusFrame);
				titleStatusLabel.Cell.Bordered = false;
				titleStatusLabel.Cell.DrawsBackground = false;
				titleStatusLabel.Cell.Editable = false;
				titleStatusLabel.Cell.TextColor = NSColor.Black;
				titleStatusLabel.Cell.Alignment = NSTextAlignment.Left;
				titleStatusLabel.Cell.Font = NSFont.SystemFontOfSize (12.0f);
				titleStatusLabel.StringValue = statusInfoStr;
				AddSubview (titleStatusLabel);
			}

			if (currencyDateTF == null) {
				currencyDateTF = new NSTextField (currencyDateFrame);
				currencyDateTF.Cell.Bordered = false;
				currencyDateTF.Cell.DrawsBackground = false;
				currencyDateTF.Cell.Editable = false;
				currencyDateTF.Cell.TextColor = Utility.ColorWithHexColorValue ("#666666",1.0f);
				currencyDateTF.Cell.Alignment = NSTextAlignment.Left;
				currencyDateTF.Cell.Font = NSFont.SystemFontOfSize (11.0f);
				currencyDateTF.ToolTip = moreStatusInfoStr;
				currencyDateTF.Cell.LineBreakMode = NSLineBreakMode.ByWordWrapping;
				currencyDateTF.Cell.TruncatesLastVisibleLine = true;
				currencyDateTF.StringValue = moreStatusInfoStr;
				AddSubview (currencyDateTF);
			}
		}

		void AddTriangleView (NSColor bkgColor, bool isShow)
		{
			if (customTriView == null) {
				var frame = new CGRect (PUBLICATION_DOWN_ORGX,
					                  PUBLICATION_DOWN_ORGY,
					                  PUBLICATION_DOWN_WIDTH,
					                  PUBLICATION_DOWN_WIDTH
				                  );

				customTriView = new TriangleView (frame);
				customTriView.BackgroudColor = bkgColor;

				WantsLayer = true;
				AddSubview (customTriView);
			}

			customTriView.Hidden = !isShow;
		}

		void AddSashView ()
		{
			HideAllDownloadView (true);

			switch (BookInfo.PublicationStatus) {
			case PublicationStatusEnum.Downloaded:
				if (BookInfo.DaysRemaining < 0) {
					ShowExpiredLabelView (true);
				} else {
					ShowDownloadBtnView (false);
				}
				break;

			case PublicationStatusEnum.NotDownloaded:
				if (BookInfo.DaysRemaining < 0) {
					ShowExpiredLabelView (true);
				} else {
						ShowDownloadBtnView (true);
				}
				break;

			case PublicationStatusEnum.RequireUpdate:
				if (BookInfo.DaysRemaining < 0) {
					ShowExpiredLabelView(true);
				} else {
					ShowUpdateBtnView (true);
				}

				break;
			}
		}
			
		void ShowDownloadBtnView (bool isShow)
		{
			if (isShow) {
				AddTriangleView (Utility.ColorWithRGB(0,128,252,1.0f), true);   //(53,88,150,1.0f)
			}

			if (downloadButton == null) {
				
				downloadButton = new NSButton (downloadButtonFrame); 
				downloadButton.Cell.Bordered = false;
				downloadButton.Cell.BezelStyle = NSBezelStyle.RegularSquare;
				downloadButton.Cell.SetButtonType (NSButtonType.MomentaryChange);
				downloadButton.Cell.ImageScale = NSImageScale.None;
				downloadButton.Cell.Image = Utility.ImageWithFilePath ("/Images/Publication/CloudInstall.png");
				downloadButton.Action = new Selector ("DownloadBook:");
				downloadButton.Target = this;

				AddSubview (downloadButton);
			}

			downloadButton.Hidden = !isShow;
		}

		void ShowUpdateBtnView(bool isShow)
		{
			if (isShow) {
				AddTriangleView (Utility.ColorWithRGB(0,128,252,1.0f), true);
			}

			if (updateButton == null) {

				updateButton = new NSButton (updateButtonFrame); 
				updateButton.Cell.Bordered = false;
				updateButton.Cell.BezelStyle = NSBezelStyle.Recessed;
				updateButton.Cell.SetButtonType (NSButtonType.MomentaryChange);
				updateButton.Cell.Alignment = NSTextAlignment.Center;

				NSAttributedString title = Utility.AttributeTitle("Update", NSColor.White, 13);
				updateButton.Cell.AttributedTitle = title;
				NSAttributedString alttitle = Utility.AttributeTitle ("Update", NSColor.Black, 12);
				updateButton.Cell.AttributedAlternateTitle = alttitle;
				updateButton.Action = new Selector ("DownloadBook:");
				updateButton.Target = this;

				nfloat rotation = updateButton.FrameCenterRotation;
				updateButton.FrameCenterRotation = rotation - 45.0f;

				AddSubview (updateButton);
			}

			updateButton.Hidden = !isShow;
		}

		void ShowExpiredBtnView (bool isShow)
		{
			if (isShow) {
				AddTriangleView (NSColor.Red, true);
			}

			if (expiredButton == null) {
				expiredButton = new NSButton (updateButtonFrame); 
				expiredButton.Cell.Bordered = false;
				expiredButton.Cell.BezelStyle = NSBezelStyle.Recessed;
				expiredButton.Cell.SetButtonType (NSButtonType.MomentaryChange);
				expiredButton.Cell.Alignment = NSTextAlignment.Center;

				NSAttributedString title = Utility.AttributeTitle("Expired", NSColor.White, 13);
				expiredButton.Cell.AttributedTitle = title;
				NSAttributedString alttitle = Utility.AttributeTitle ("Expired", NSColor.Black, 12);
				expiredButton.Cell.AttributedAlternateTitle = alttitle;
				expiredButton.Action = new Selector ("ShowRenewBookAlert:");
				expiredButton.Target = this;

				nfloat rotation = expiredButton.FrameCenterRotation;
				expiredButton.FrameCenterRotation = rotation - 45.0f;

				AddSubview (expiredButton);
			}

			expiredButton.Hidden = !isShow;
		}

		[Action("ShowRenewBookAlert:")]
		void ShowRenewBookAlert (NSObject sender)
		{
			//Console.WriteLine ("ShowRenewBookAlert");
			AlertSheet.RunPromptAlert ("Prompt","how to renew the title.");
		}

		//isShow:false progressview:hide
		//isShow:true  progressview:show
		public void ShowSashView (bool isShow)
		{
			if (!isShow) {
				if (downloadPreUpdate) {
					ShowUpdateBtnView (true);
				} else {
					ShowDownloadBtnView (true);
				}
			} else {
				if (downloadButton != null) {
					if (!downloadButton.Hidden) {
						downloadButton.Hidden = true;
						downloadPreUpdate = false;
					}
				}

				if (updateButton != null) {
					if (!updateButton.Hidden) {
						updateButton.Hidden = true;
						downloadPreUpdate = true;
					}
				}
			}

			if (progressView == null) {
				progressView = new CircularProgressView (progressViewFrame);
				progressView.superViewObj = this;
				AddSubview (progressView);
			}
				
			progressView.ResetProgress ();
			progressView.Hidden = !isShow;
		}

		void ShowExpiredLabelView (bool isShow)
		{
			if (isShow) {
				AddTriangleView (NSColor.Red, true);
			}

			if (updateInfoLabel == null)
			{
				var updateButtonFrame = new CGRect (-10, -48, PUBLICATION_DOWN_WIDTH, PUBLICATION_DOWN_ORGX);
				updateInfoLabel = new NSTextField (updateButtonFrame);
				updateInfoLabel.Cell.Bordered = false;
				updateInfoLabel.Cell.DrawsBackground = false;
				updateInfoLabel.Cell.Editable = false;
				updateInfoLabel.Cell.TextColor = NSColor.White;
				updateInfoLabel.Cell.Alignment = NSTextAlignment.Left;
				updateInfoLabel.Cell.Font = NSFont.SystemFontOfSize (15.0f);
				updateInfoLabel.StringValue = "Expired";

				nfloat rotation = updateInfoLabel.FrameCenterRotation;
				updateInfoLabel.FrameCenterRotation = rotation - 45.0f;

				if (customTriView != null) {
					customTriView.WantsLayer = true;
					customTriView.AddSubview (updateInfoLabel);
				}
			}

			updateInfoLabel.Hidden = !isShow;
		}

		void HideAllDownloadView(bool isHide)
		{
			if (customTriView != null) {
				customTriView.Hidden = isHide;
			}

			if (downloadButton != null) {
				downloadButton.Hidden = isHide;
			}

			if (updateButton != null) {
				updateButton.Hidden = isHide;
			}

			if (updateInfoLabel != null) {
				updateInfoLabel.Hidden = isHide;
			}

			if ( progressView!= null) {
				progressView.Hidden = isHide;
			}
		}

		void AddInfoButtonFrame ()
		{
			if (infoButton == null) {
				infoButton = new NSButton (infoButtonFrame);
				infoButton.BezelStyle = NSBezelStyle.RegularSquare;
				infoButton.SetButtonType (NSButtonType.MomentaryPushIn);
				infoButton.Title = "Info";
				infoButton.Action = new Selector ("InfoButtonClick:");
				infoButton.Target = this;
				AddSubview (infoButton);
			}

			switch (BookInfo.PublicationStatus) {
			case PublicationStatusEnum.Downloaded:
				infoButton.Enabled = true;
				break;

			case PublicationStatusEnum.NotDownloaded:
				infoButton.Enabled = false;
				break;

			case PublicationStatusEnum.RequireUpdate:
				infoButton.Enabled = true;
				break;
			}
		}
		#endregion

		#region mark api with DownloadManager
		public void CancelDownload ()
		{
			DownloadController.CancelDownload ();
		}

		public void UpdateDownloadProgress(int bytesDownloaded, long downloadSize)
		{
			this.InvokeOnMainThread(()=>{
				float curProgress = (float)(bytesDownloaded/100.0);
				if (infoController != null) {
					infoController.UpdateDownloadProgress (bytesDownloaded, downloadSize);
				} else {
					progressView.UpdateProgress (curProgress);
				}
			});
		}

		public void UpdateDownloadStatus (DownloadResult downloadResult, bool isUpdate)
		{
			if (infoController != null) {
				infoController.RefreshDownloadStatus (downloadResult, isUpdate);
			} else {
				RefreshDownloadStatus (downloadResult, isUpdate);
			}
		}

		private void RefreshDownloadStatus (DownloadResult downloadResult, bool isUpdate)
		{
			UpdateStatus = 0;
			string status = String.Empty;
			switch (downloadResult.DownloadStatus) {
			case DownLoadEnum.Canceled:
				if (isUpdate) {
					status = "Update failed";
				} else {
					status = "Download failed";
				}
				downloadPreUpdate = isUpdate;
				ShowSashView (false);
				AlertSheet.DestroyConfirmAlert ();
				break;

			case DownLoadEnum.Failure:
				if (isUpdate) {
					status = "Update failed";
				} else {
					status = "Download failed";
				}
				downloadPreUpdate = isUpdate;
				ShowSashView (false);
				AlertSheet.DestroyConfirmAlert ();
				break;

			case DownLoadEnum.NetDisconnected:
				status = "Missing connection";
				downloadPreUpdate = isUpdate;
				ShowSashView (false);

				if (!Utility.IsKeyWindowLogin()) {
					AlertSheet.ShowMissingConnectAlert ();
				}
				break;

			case DownLoadEnum.Success:
				BookInfo = downloadResult.Publication;
				PublicationsDataManager.SharedInstance.ReplacePublicationByBookID (BookInfo);
				PublicationsDataManager.SharedInstance.CurrentPublication = BookInfo;

				UpdateTitleStatus ();
				var winController = (PublicationsWindowController)Window.WindowController;
				winController.PublicationsVC.RefreshHistoryView ();

				AlertSheet.DestroyConfirmAlert ();

				NSNotificationCenter.DefaultCenter.PostNotificationName(LNRConstants.LNPublicationDidFinishedDownload,
					NSObject.FromObject(BookInfo.BookId));
				
				break;
			}

			if (status.Length > 0) {

				NSAttributedString attributeTitle = Utility.AttributedTitle(status, 
					Utility.ColorWithHexColorValue ("#000000",1.0f), "System",12.0f,NSTextAlignment.Left);
				titleStatusLabel.AttributedStringValue = attributeTitle;
			}

			if (downloadResult.DownloadStatus == DownLoadEnum.Failure) {
				if (!Utility.IsKeyWindowLogin ()) {
					AlertSheet.RunPromptAlert ("Installation Error", "Installation of the book has been interrupted. Please re-install the book.");
				}
			}
		}

		private void UpdateTitleStatus ()
		{
			titleStatusLabel.StringValue = "Up to date";
			currencyDateTF.StringValue = "Currency Date " + BookInfo.CurrencyDate.Value.ToString ("dd MMM yyyy");
			progressView.UpdateProgress (1.0f);
			infoButton.Enabled = true;
			HideAllDownloadView (true);
		}
		#endregion

		#region Action
		[Action("DownloadBook:")]
		async void DownloadBook (NSObject sender)
		{
			ShowSashView (true);
			bool isUpdate = false;
			var button = (NSButton)sender;
			if (button.Title == "Update") {
				isUpdate = true;
				UpdateStatus = 1;
			}

			await DownloadController.StartDownloadWithoutLimitation (BookInfo.BookId, isUpdate);
		}
		#endregion

		#region mark deal with update from info modal
		public void HandleFinishDownload(NSNotification notification) 
		{
			var bookID = Int32.Parse(notification.Object.ToString());
			if (bookID == BookInfo.BookId) {
				BookInfo = PublicationsDataManager.SharedInstance.CurrentPublication;
				HideAllDownloadView (true);
				titleStatusLabel.StringValue = "Up to date";
				currencyDateTF.StringValue = "Currency Date " + BookInfo.CurrencyDate.Value.ToString ("dd MMM yyyy");
			}
		}
			
		public void RefreshUpdateStatus (int status)
		{
			if (status == 1) {
				UpdateDownloadProgress ();
			} else if (status == 2){
				RefreshPublicationView ();
			}else if (status == 3){
				downloadPreUpdate = true;
				ShowSashView (false);
			}
		}
		#endregion

		[Action("InfoButtonClick:")]
		private async void InfoButtonClick (NSObject sender)
		{
			isCanToContent = true;
			CGPoint orgPoint = Utility.GetModalPanelLocation(690.0f, LNRConstants.WindowHeight_MIN);  //658+22

			using (var infoPanelController = new PublicationInfoPanelController (orgPoint,this)) {
				infoController = infoPanelController;
				infoPanelController.BookInfo = BookInfo;
				infoPanelController.InitializeInfoView (UpdateStatus);
				var infoPanel = infoPanelController.Window;
				infoPanel.MakeFirstResponder (null);

				NSApplication NSApp = NSApplication.SharedApplication;

				infoPanel.WindowShouldClose += (t) => true;
				bool isOpenContentPanel = false;

				infoPanel.WillClose += delegate(object asender, EventArgs e) {
					isOpenContentPanel = infoPanelController.IsOpenContentPanel;
					UpdateStatus = infoPanelController.UpdateStatus;
					NSApp.StopModal ();
				};

				NSApp.RunModalForWindow (infoPanel);
				infoPanelController.Window.OrderOut (null);

				RefreshUpdateStatus (UpdateStatus);

				if (isOpenContentPanel) {
					await OpenContentPanel ();
				}
			}
			infoController = null;
		}



		//exit from info modal, the publicationview must be refreshed
		private void UpdateDownloadProgress ()
		{
			ShowSashView (true);
		}

		private void RefreshPublicationView ()
		{
			Publication currentBook = PublicationsDataManager.SharedInstance.CurrentPublication;
			if (currentBook != null && currentBook != BookInfo) {
				BookInfo = PublicationsDataManager.SharedInstance.CurrentPublication;
				HideAllDownloadView (true);
				titleStatusLabel.StringValue = "Up to date";
				currencyDateTF.StringValue = "Currency Date " + BookInfo.CurrencyDate.Value.ToString ("dd MMM yyyy");
			}
		}

		//exit from info modal, swith to content window,
		private async Task OpenContentPanel ()
		{
			PublicationsDataManager.SharedInstance.CurrentPublication = BookInfo;
			PublicationsDataManager.SharedInstance.CurrentPublicationView = this;

			NSWindow mainWindow = Superview.Superview.Window;
			PublicationsWindowController windowController = (PublicationsWindowController)mainWindow.WindowController;
			if (windowController != null) {
				await windowController.SwitchToContentView (-1);
			}
		}

		public async override void MouseUp (NSEvent theEvent)
		{
			//base.MouseUp (theEvent);

			if (isCanToContent) {
				isCanToContent = false;
				return;
			}

			if (infoButton.Enabled) {
				await OpenContentPanel ();
			}
		}

		public override void DrawRect (CGRect dirtyRect)
		{
			base.DrawRect (dirtyRect);

			NSGraphicsContext.GlobalSaveGraphicsState();

			//backgound frame
			DrawCoverRect();

			//title frame
			DrawTitleRect();

			//bottom line
			DrawBottomLine();

			NSGraphicsContext.GlobalRestoreGraphicsState();

		}
			
		//
		//
		void DrawCoverRect ()
		{
			Utility.ColorWithHexColorValue (BookInfo.ColorPrimary,1.0f).Set ();
			NSGraphics.RectFill (coverFrame);
		}

		void DrawTitleRect ()
		{
			Utility.ColorWithHexColorValue (BookInfo.ColorSecondary,1.0f).Set ();
			NSGraphics.RectFill (titleBorderFrame);

			var oPath = NSBezierPath.FromRect (wideBorderLineFrame);
			Utility.ColorWithHexColorValue (BookInfo.FontColor,1.0f).Set();
			oPath.LineWidth = 3;
			oPath.Stroke ();

			var iPath = NSBezierPath.FromRect (thinBorderLineFrame);
			Utility.ColorWithHexColorValue (BookInfo.ColorSecondary,1.0f).Set();
			iPath.LineWidth = 1;
			iPath.Stroke ();
		}

		//no use
		void DrawUpdateView()
		{
			if (BookInfo.DaysRemaining < 0) {
				NSColor.Red.Set ();
			} else {
				Utility.ColorWithRGB(0,128,252,1.0f).Set();
			}


			CGContext context = NSGraphicsContext.CurrentContext.GraphicsPort;
			context.BeginPath ();
			context.MoveTo (PUBLICATION_DOWN_ORGX,PUBLICATION_VIEW_HEIGHT);

			context.AddLineToPoint (PUBLICATION_COVER_WIDTH, PUBLICATION_DOWN_ORGY);
			context.AddLineToPoint (PUBLICATION_COVER_WIDTH, PUBLICATION_VIEW_HEIGHT);
			context.AddLineToPoint (PUBLICATION_DOWN_ORGX, PUBLICATION_VIEW_HEIGHT);
			context.ClosePath ();

			context.DrawPath (CGPathDrawingMode.Fill);
		}
		//

		void DrawBottomLine ()
		{
			var linePath = new NSBezierPath ();

			Utility.ColorWithHexColorValue (BookInfo.FontColor,1.0f).SetStroke ();

			linePath.LineWidth = 2;
			linePath.MoveTo (bottomLineTS);
			linePath.LineTo(bottomLineTE);
			linePath.ClosePath ();

			linePath.Stroke ();

			//var lineTPath = new NSBezierPath ();
			linePath.MoveTo (bottomLineBS);
			linePath.LineTo(bottomLineBE);
			linePath.ClosePath ();
			linePath.Stroke ();
		}


		//
		static CGRect coverFrame = new CGRect (0,
													   PUBLICATION_COVER_ORGY,
													   PUBLICATION_COVER_WIDTH,
													   PUBLICATION_COVER_HEIGHT);

		//
		static CGRect wideBorderLineFrame = new CGRect (PUBLICATION_COVERLINE_SPACE,
																PUBLICATION_TITLE_ORGY,
																PUBLICATION_LINT_WIDTH,
																PUBLICATION_LINT_HEIGHT);


		static CGRect thinBorderLineFrame = new CGRect (PUBLICATION_COVERLINE_SPACE,
																PUBLICATION_TITLE_ORGY,
																PUBLICATION_LINT_WIDTH,
																PUBLICATION_LINT_HEIGHT);

		//
		static CGRect titleBorderFrame = new CGRect (PUBLICATION_COVERLINE_SPACE+2,
															 PUBLICATION_TITLE_ORGY+2,
															 PUBLICATION_LINT_WIDTH-4,
															 PUBLICATION_LINT_HEIGHT-4);

		static CGRect TitleFrameWithTitle (string title)
		{
			NSFont newFont = NSFont.FromFontName ("Garamond", 17.5f);
			nfloat textHeight = HeightWrappedToWidth (title, newFont);
			nfloat textHeights = HeightWrappedWidth (newFont, title);
			//Console.WriteLine ("title:{2},textHeight:{0};textHeights:{1}",textHeight,textHeights,title);
			textHeight = textHeight == (textHeights - 5) ? textHeight : textHeights - 5;
			if (textHeight > 100) {
				textHeight = 100;
			}
			return new CGRect (PUBLICATION_COVERLINE_SPACE + 6,
				PUBLICATION_TITLE_ORGY + (PUBLICATION_LINT_HEIGHT/2 - textHeight/2),
				PUBLICATION_LINT_WIDTH - 6 * 2,
				textHeight);
				
		}

		static CGRect TitleCaseFrameWithTitle (string title)
		{
			NSFont newFont = NSFont.FromFontName ("Garamond", 17.5f);
			nfloat textHeight = HeightWrappedToWidth (title, newFont);
			if (textHeight > 75) {
				textHeight = 75;
			}

			return new CGRect (PUBLICATION_COVERLINE_SPACE + 8,
				PUBLICATION_TITLE_ORGY + (PUBLICATION_LINT_HEIGHT/2 - textHeight/2)+PUBLICATION_TITLE_LINTHEIGHT/2,
				PUBLICATION_LINT_WIDTH - 8 * 2,
				textHeight);

		}

		static nfloat HeightWrappedToWidth (string title, NSFont font)
		{
			NSAttributedString textAttrStr = new NSAttributedString (title, font);
			CGSize maxSize = new CGSize (PUBLICATION_LINT_WIDTH - 6 * 2, 1000);
			CGRect boundRect = textAttrStr.BoundingRectWithSize (maxSize,
				NSStringDrawingOptions.TruncatesLastVisibleLine | 
				NSStringDrawingOptions.UsesLineFragmentOrigin | 
				NSStringDrawingOptions.UsesFontLeading);

			//multiple of 17
			nfloat stringHeight = boundRect.Height;
			return stringHeight;
		}

		static nfloat HeightWrappedWidth (NSFont font, string title)
		{
			NSTextField textField = new NSTextField(new CGRect(0, 0, PUBLICATION_LINT_WIDTH - 6 * 2, 1000));
			textField.Font = font;
			textField.StringValue = title;

			CGSize size = textField.Cell.CellSizeForBounds(textField.Frame);
			return size.Height;
		}

		static CGRect downloadButtonFrame = new CGRect (PUBLICATION_DOWN_ORGX+PUBLICATION_DOWN_WIDTH/2+2,
																PUBLICATION_DOWN_ORGY+PUBLICATION_DOWN_WIDTH/2+2,
																30,29);

		static CGRect progressViewFrame = new CGRect (PUBLICATION_DOWN_ORGX+PUBLICATION_DOWN_WIDTH/2+4,
			PUBLICATION_DOWN_ORGY+PUBLICATION_DOWN_WIDTH/2+4,28,28);

		static CGRect updateButtonFrame = new CGRect (PUBLICATION_DOWN_ORGX+PUBLICATION_DOWN_WIDTH/2-18,
															  PUBLICATION_DOWN_ORGY+PUBLICATION_DOWN_WIDTH/2+4,
															  56,20);
			
		//
		static CGRect loanFrame = new CGRect (PUBLICATION_COVERLINE_SPACE,
			                                          PUBLICATION_LOAN_ORGY,
													  PUBLICATION_LINT_WIDTH,
													  PUBLICATION_TITLE_LINTHEIGHT);

		static CGRect loanDateFrame = new CGRect (PUBLICATION_COVERLINE_SPACE,
														  PUBLICATION_LOANDAY_ORGY,
														  PUBLICATION_LINT_WIDTH,
														  PUBLICATION_TITLE_LINTHEIGHT);
			
		//
		static CGPoint bottomLineTS = new CGPoint (PUBLICATION_COVERLINE_SPACE, 
			                                     PUBLICATION_COVER_ORGY+3+PUBLICATION_COVERLINE_SPACE);

		static CGPoint bottomLineTE = new CGPoint (PUBLICATION_COVER_WIDTH-PUBLICATION_COVERLINE_SPACE, 
			                                     PUBLICATION_COVER_ORGY+3+PUBLICATION_COVERLINE_SPACE);

		static CGPoint bottomLineBS = new CGPoint (PUBLICATION_COVERLINE_SPACE, 
			                                     PUBLICATION_COVER_ORGY+PUBLICATION_COVERLINE_SPACE);

		static CGPoint bottomLineBE = new CGPoint (PUBLICATION_COVER_WIDTH-PUBLICATION_COVERLINE_SPACE, 
				                                 PUBLICATION_COVER_ORGY+PUBLICATION_COVERLINE_SPACE);


		//
		static CGRect titleStatusFrame = new CGRect (2, 54, PUBLICATION_COVER_WIDTH-4, 16);

		static CGRect currencyDateFrame = new CGRect (2, 38, PUBLICATION_LINT_WIDTH-4, 14);

		//
		static CGRect infoButtonFrame = new CGRect (0,5,PUBLICATION_COVER_WIDTH,28);

		//for test
		void AddTimer ()
		{
			var timeout = new TimeSpan ((long)(((0.1 * TimeSpan.TicksPerSecond) / 1) + 0.0));
			int progress = 10;
			long downloadSize = 0;
			NSTimer timer = NSTimer.CreateRepeatingScheduledTimer(timeout, 
				delegate {
					if (progress<=100) {
						//Console.WriteLine("{0}",progress);
						UpdateDownloadProgress(progress, downloadSize);
						progress += 10;
					} else {
					}
				});

			NSRunLoop.Current.AddTimer (timer, NSRunLoopMode.Default);
		}

		protected override void Dispose (bool disposing)
		{
			BookInfo = null;

			customTriView = null; 
			downloadButton = null;
			updateButton = null;
			expiredButton = null;
			updateInfoLabel = null;

			progressView = null;

			//
			currencyDateTF = null;
			titleStatusLabel = null;
			infoButton = null;
		}
		#if false
		async void InstallCurrentBook ()
		{
			var cancelTokenSource = new CancellationTokenSource ();

			InstallResultEnum installResult = await PublicationUtil.Instance.Install (BookInfo.BookId, cancelTokenSource.Token);

			switch (installResult) {
			case InstallResultEnum.Cancel:
				ShowInstallErrorAlert ();
				break;

			case InstallResultEnum.Failure:
				ShowInstallErrorAlert ();
				break;

			case InstallResultEnum.Success:
				break;
			}
		}
		#endif

		nint ShowInstallErrorAlert ()
		{
			return AlertSheet.RunPromptAlert ("Installation Error", "Installation of the book has been interrupted. Please re-install the book.");
		}
	}
}

