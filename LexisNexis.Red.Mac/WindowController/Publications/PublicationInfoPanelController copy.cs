
using System;
using System.Collections.Generic;

using Foundation;
using AppKit;
using ObjCRuntime;

using LexisNexis.Red.Common.BusinessModel;
using CoreGraphics;
using LexisNexis.Red.Common.Business;
using System.Threading;
using LexisNexis.Red.Mac.Data;

namespace LexisNexis.Red.Mac
{
	public partial class PublicationInfoPanelController : NSWindowController
	{
		#region Properties
		public NSImage bookCoverImage {get; set;}
		public Publication BookInfo { get; set;}
		bool isDescripExpand { get; set; }
		bool isWhatsNewExpand { get; set; }
		public bool IsOpenContentPanel {get; set;}
		public bool IsUpdateFinished { get; set;}
		NSRange despRange { get; set; }
		uint lineNumber { get; set; }
		bool isFTC;
		//CircularProgressView progressView;

		const float VERTICAL_SPACING = 5;
		const float CELL_VERTICAL_SPACEING = 3;
		const float GUIDE_TEXTFIELD_HEIGHT = 18;
		const float TEXTVIEW_SIXLINE_HEIGHT = 104;

		static nfloat expiredViewMinHeight;
		static nfloat expiredViewMaxHeight;
		bool isExpiredExpand { get; set;}

		static nfloat descriptionViewHeight;
		//static nfloat descriptionViewMaxHeight;

		static nfloat whatsNewViewMinHeight;
		static nfloat whatsNewViewMaxHeight;
		static nfloat whatsNewInfoHeight;
		static CGPoint whatsNewInfoOrgPoint;
		enum GuideCardMode {
			Added = 0,
			Deleted = 1,
			Updated = 2
		} ;

		int bytesPreDownloaded;
		CancellationTokenSource tokenSource;

		//strongly typed window accessor
		public new PublicationInfoPanel Window {
			get {
				return (PublicationInfoPanel)base.Window;
			}
		}

		#endregion

		#region Constructors

		// Called when created from unmanaged code
		public PublicationInfoPanelController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public PublicationInfoPanelController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public PublicationInfoPanelController (CGPoint location) : base ("PublicationInfoPanel")
		{
			Window.AnimationBehavior = NSWindowAnimationBehavior.Default;
			Window.SetFrameOrigin (location);
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
			Window.Title = "Book Details";
			//Window.Delegate = new WindowDelegate ();
			Window.BackgroundColor = NSColor.White;
			Window.MakeFirstResponder (null);

			ScrollView.ScrollerKnobStyle = NSScrollerKnobStyle.Default;
			ScrollView.VerticalScroller.ControlSize = NSControlSize.Small;

		}

		#endregion


		#region methods
		public void InitializeInfoView()
		{
			isFTC = BookInfo.IsFTC;

			//
			InitializeCoverView ();

			RefreshInfoView ();
		}

		void InitializeCasesView ()
		{
			if (isFTC) {
				CaseView.Hidden = false;
			} else {
				CaseView.Hidden = true;
			}
		}

		void InitializeCoverView ()
		{
			if (BookInfo == null) {
				return;
			}

			CGRect viewFrame = CoverImageView.Frame;
			var coverFrame = new CGRect (0, 0, viewFrame.Size.Width, viewFrame.Size.Height);

			var imageView = new BookCoverView (coverFrame);
			imageView.ColorPrimary = BookInfo.ColorPrimary;
			imageView.ColorSecondary = BookInfo.ColorSecondary;
			imageView.FontColor = BookInfo.FontColor;
			imageView.Title = BookInfo.Name;
			imageView.DaysRemaining = BookInfo.DaysRemaining;
			imageView.IsLoan = BookInfo.IsLoan;
			imageView.IsFTC = isFTC;

			imageView.InitializeValue (0);
			CoverImageView.AddSubview (imageView);

			if (BookInfo.DaysRemaining < 0) {

				//float rate = viewFrame.Size.Width / viewFrame.Size.Height;
				float rate = 200.0f / 270.0f;
				float width = 90 * rate + 3;
				nfloat orgx = viewFrame.Size.Width - width;
				nfloat orgy = viewFrame.Size.Height - width;
				var expiredSashView = new NSImageView (new CGRect (orgx, orgy, width, width));
				expiredSashView.Image = Utility.ImageWithFilePath ("/Images/Publication/Expired-Sash.png");
				expiredSashView.ImageScaling = NSImageScale.AxesIndependently;
				CoverImageView.AddSubview (expiredSashView);
			}

			ProgressView.Hidden = true;

			if (BookInfo.DaysRemaining < 0) {
				UpdateButton.Title = "Open";
				UpdateButton.Enabled = true;
			} else {
				UpdateButton.Title = "Update";
				if (BookInfo.PublicationStatus == PublicationStatusEnum.RequireUpdate) {
					UpdateButton.Enabled = true;
				} else {
					UpdateButton.Title = "Open";
					UpdateButton.Enabled = true;
				}
			}
		}

		void InitailizeTitleInfo ()
		{
			if (BookInfo == null) {
				return;
			}

			if (BookInfo.Name != null) {
				BookTitleLabel.StringValue = BookInfo.Name;
				BookTitleLabel.ToolTip = BookInfo.Name;
			}

			if (BookInfo.Author!=null) {
				AuthorLabel.StringValue = BookInfo.Author;
				AuthorLabel.ToolTip = BookInfo.Author;
			}

			if (BookInfo.IsLoan) {
				SetButtonAttributeTitle (LoanButton, "Loan");
				LoanButton.Hidden = false;
			} else {
				LoanButton.Hidden = true;
			}

			if (isFTC) {
				SetButtonAttributeTitle (CasesButton, "+Cases");
				if (LoanButton.Hidden) {
					CasesButton.SetFrameOrigin (LoanButton.Frame.Location);
				}
				CasesButton.Hidden = false;
			} else {
				CasesButton.Hidden = true;
			}
		}

		void SetButtonAttributeTitle (NSButton button, string title)
		{
			button.Cell.Bordered = false;
			button.WantsLayer = true;
			button.Layer.BackgroundColor = Utility.ColorWithRGB (132, 132, 132).CGColor; //132
			button.Layer.CornerRadius = 8.0f;
			button.Cell.AttributedTitle = Utility.AttributeTitle (title, 
				Utility.ColorWithHexColorValue ("#ececec", 1.0f), 12);  //236
			button.Cell.AttributedAlternateTitle = Utility.AttributeTitle ("Loan", 
				Utility.ColorWithHexColorValue ("#ececec", 1.0f), 12);
		}

		void InitalizeExpiredView()
		{
			ContactEmailLabel.AttributedTitle = Utility.AttributeTitle ("customersupport@lexisnexis.com.au",
				NSColor.Blue, 14);
			ContactEmailLabel.AttributedAlternateTitle = Utility.AttributeTitle ("customersupport@lexisnexis.com.au",
				Utility.ColorWithRGB (77, 151, 250), 14);

			if (BookInfo == null) {
				return;
			}

			if (BookInfo.DaysRemaining >= 0) {
				ExpiredView.Hidden = true;
			} else {
				ExpiredView.Hidden = false;
			}

			float fontSize = 13.0f;
			string description = "We would like to thank you for choosing LexisNexis to support the great work you do on a daily basis, " +
				"and we hope to continue being your legal content provider of choice.\n"+
				"To re-subscribe to this content and continue receiving updates please contact your Relationship Manager, " +
				"or Customer service on 1800 772 772 or customersupport@lexisnexis.com.au. \n"+
				"you can tell your command."; 

			ExpiredInfoTF.AttributedStringValue = Utility.AttributeLinkTitle (description, 
				"customersupport@lexisnexis.com.au", Utility.ColorWithHexColorValue ("#666666",0.85f), fontSize);

			//ExpiredInfoTF.StringValue = description;
		
			nfloat orgHeight = ExpiredInfoTF.Frame.Size.Height;
			nfloat height = HeightWrappedToWidth (ExpiredInfoTF,fontSize);
			nfloat offset = height - orgHeight;

			var oldSize = ExpiredView.Frame.Size;
			expiredViewMinHeight = oldSize.Height;
			expiredViewMaxHeight = oldSize.Height + offset;

			if (height > TEXTVIEW_SIXLINE_HEIGHT) {
				NSButton moreButton = CreateMoreButton ();
				ExpiredView.WantsLayer = true;
				moreButton.Action = new Selector ("ExpiredViewShowAll:");
				moreButton.Target = this;
				ExpiredView.AddSubview (moreButton);

				isExpiredExpand = false;
			} else {
				var newSize = new CGSize (oldSize.Width, expiredViewMinHeight+offset);
				ExpiredView.SetFrameSize (newSize);
			}
		}

		[Action("ExpiredViewShowAll:")]
		void ExpiredViewShowAll(NSObject sender)
		{
			float fontSize = 12.0f;
			if (!isExpiredExpand) {
				var oldSize = ExpiredView.Frame.Size;
				var newSize = new CGSize (oldSize.Width, expiredViewMaxHeight);
				ExpiredView.SetFrameSize (newSize);

				var button = (NSButton)ExpiredView.Subviews [6]; //对照xib 确定id
				NSAttributedString title = Utility.AttributeTitle("...less", Utility.ColorWithRGB(44,117,252), fontSize);
				button.Cell.AttributedTitle = title;

				isExpiredExpand = true;

			} else {
				var oldSize = ExpiredView.Frame.Size;
				var newSize = new CGSize (oldSize.Width, expiredViewMinHeight);
				ExpiredView.SetFrameSize (newSize);

				var button = (NSButton)ExpiredView.Subviews [6];
				NSAttributedString title = Utility.AttributeTitle("...more", Utility.ColorWithRGB(44,117,252), fontSize);
				button.Cell.AttributedTitle = title;

				isExpiredExpand = false;
			}

			LayoutSubViews ();
		}

		partial void ContactEmail (NSObject sender)
		{
			NSWorkspace workSpace = NSWorkspace.SharedWorkspace;
			String mailtoAddress = String.Format("mailto:{0}?Subject={1}&body={2}",
				"customersupport@lexisnexis.com.au","Expired","Support");

			workSpace.OpenUrl(NSUrl.FromString(mailtoAddress));
		}

		void InitializeDescriptionView ()
		{
			if (BookInfo == null) {
				return;
			}

			float fontSize = 13.0f;

			string description = "3. Verify return to filter option page for Subscription " +
				"Given successful login with navigation to My Publications page, " +
				"When filtering books by 'Subscription' option \n" +
				"and selecting a Subscription title using TOC icon, " +
				"Then returning back to My Publications screen will maintain the list of filtered titles.338\n" +
				"When clicking the download icon on the subscribed download book, " +
				"showing the download progress and a cancel icon at end of progress bar\n" +
				"3. Cancel download progress this is";
			
			DespTextField.StringValue = description;
			if (BookInfo.Description != null) {
				DespTextField.StringValue = BookInfo.Description;
			}

			descriptionViewHeight = DescriptionView.Frame.Size.Height;

			var height = HeightWrappedToWidth (DespTextField,fontSize);

			if (height > TEXTVIEW_SIXLINE_HEIGHT) {

				NSButton moreButton = CreateMoreButton ();
				DescriptionView.WantsLayer = true;
				moreButton.Action = new Selector ("DescriptionShowAll:");
				moreButton.Target = this;
				DescriptionView.AddSubview (moreButton);

				isDescripExpand = false;
			} else {
				nfloat orgHeight = DespTextField.Frame.Size.Height;
				nfloat offset = height - orgHeight;
				var oldSize = DescriptionView.Frame.Size;
				var newSize = new CGSize (oldSize.Width, descriptionViewHeight+offset);
				DescriptionView.SetFrameSize (newSize);
				//LayoutSubViews ();
			}

		}
	
		[Action("DescriptionShowAll:")]
		void DescriptionShowAll(NSObject sender)
		{
			float fontSize = 13.0f;
			nfloat height = HeightWrappedToWidth (DespTextField,fontSize);

			if (!isDescripExpand) {
				nfloat orgHeight = DespTextField.Frame.Size.Height;
				nfloat offset = height - orgHeight;
				var oldSize = DescriptionView.Frame.Size;
				var newSize = new CGSize (oldSize.Width, descriptionViewHeight+offset);
				DescriptionView.SetFrameSize (newSize);

				var button = (NSButton)DescriptionView.Subviews [3];
				NSAttributedString title = Utility.AttributeTitle("...less", Utility.ColorWithRGB(44,117,252), 12);
				button.Cell.AttributedTitle = title;

				isDescripExpand = true;

			} else {
				var oldSize = DescriptionView.Frame.Size;
				var newSize = new CGSize (oldSize.Width, descriptionViewHeight);
				DescriptionView.SetFrameSize (newSize);

				var button = (NSButton)DescriptionView.Subviews [3];
				NSAttributedString title = Utility.AttributeTitle("...more", Utility.ColorWithRGB(44,117,252), 12);
				button.Cell.AttributedTitle = title;

				isDescripExpand = false;
			}

			LayoutSubViews ();
		}

		//
		nfloat HeightOfWhatNewInfoView ()
		{
			List<GuideCard> addedGuideCardList = BookInfo.AddedGuideCard;
			List<GuideCard> delGuideCardList = BookInfo.DeletedGuideCard;
			List<GuideCard> updateGuideCardList = BookInfo.UpdatedGuideCard;
			string addString = "GuideCards Added:\n";
			if (addedGuideCardList.Count > 0) {
				foreach (GuideCard guidCard in addedGuideCardList) {
					if (!string.IsNullOrEmpty (guidCard.Name)) {
						addString += guidCard.Name + "\n";
					}
				}
			} else {
				addString += "No Files Added";
			}

			string deleteString = "GuideCards Deleted:\n";
			if (delGuideCardList.Count > 0) {
				foreach (GuideCard guidCard in delGuideCardList) {
					if (!string.IsNullOrEmpty (guidCard.Name)) {
						deleteString += guidCard.Name + "\n";
					}
				}
			} else {
				deleteString += "No Files Deleted";
			}

			string updateString = "GuideCards Updated:\n";
			if (updateGuideCardList.Count > 0) {
				foreach (GuideCard guidCard in updateGuideCardList) {
					if (!string.IsNullOrEmpty (guidCard.Name)) {
						updateString += guidCard.Name + "\n";
					}
				}
			} else {
				updateString += "No Files Updated";
			}
			String infoString = addString+deleteString+updateString;
			WhatNewInfoLabel.StringValue = infoString;

			NSAttributedString textAttrStr = new NSAttributedString (infoString,NSFont.SystemFontOfSize(13));
			CGSize maxSize = new CGSize (430, 1000);
			CGRect boundRect = textAttrStr.BoundingRectWithSize (maxSize,
				NSStringDrawingOptions.TruncatesLastVisibleLine |
				NSStringDrawingOptions.UsesLineFragmentOrigin| 
				NSStringDrawingOptions.UsesFontLeading);

			//multiple of 17
			nfloat stringHeight = boundRect.Height;

			return stringHeight;
		}

		void InitializeWhatNewView ()
		{
			if (BookInfo == null) {
				return;
			}
			WhatNewLabel.StringValue = "What's New";
			CurrentDayLabel.StringValue = BookInfo.LastUpdatedDate.Value.ToString("dd MMM yyyy");
			nfloat height = HeightOfWhatNewInfoView ();
			if (height > TEXTVIEW_SIXLINE_HEIGHT) {

				NSButton moreButton = CreateMoreButton ();
				DescriptionView.WantsLayer = true;
				moreButton.Action = new Selector ("WhatNewViewShowAll:");
				moreButton.Target = this;
				DescriptionView.AddSubview (moreButton);

				isDescripExpand = false;
			} else {
				nfloat orgHeight = DespTextField.Frame.Size.Height;
				nfloat offset = height - orgHeight;
				var oldSize = DescriptionView.Frame.Size;
				var newSize = new CGSize (oldSize.Width, descriptionViewHeight+offset);
				DescriptionView.SetFrameSize (newSize);
				//LayoutSubViews ();
			}
		}

		[Action("WhatNewViewShowAll:")]
		void WhatNewViewShowAll(NSObject sender)
		{
			var oldSize = WhatNewView.Frame.Size;
			if (!isWhatsNewExpand) {
				var newSize = new CGSize (oldSize.Width, whatsNewViewMaxHeight);
				WhatNewView.SetFrameSize (newSize);

				CGPoint orgPoint = WhatNewInfoView.Frame.Location;
				WhatNewInfoView.SetFrameOrigin (new CGPoint(orgPoint.X,5));

				var button = (NSButton)WhatNewView.Subviews [4];
				NSAttributedString title = Utility.AttributeTitle("...less", Utility.ColorWithRGB(44,117,252), 12);
				button.Cell.AttributedTitle = title;

				isWhatsNewExpand = true;

			} else {
				var newSize = new CGSize (oldSize.Width, whatsNewViewMinHeight);
				WhatNewView.SetFrameSize (newSize);

				CGPoint orgPoint = whatsNewInfoOrgPoint;
				WhatNewInfoView.SetFrameOrigin (orgPoint);

				var button = (NSButton)WhatNewView.Subviews [4];
				NSAttributedString title = Utility.AttributeTitle("...more", Utility.ColorWithRGB(44,117,252), 12);
				button.Cell.AttributedTitle = title;

				isWhatsNewExpand = false;
			}

			LayoutSubViews ();
		}

		#if false
		void InitializeWhatNewView ()
		{
			if (BookInfo == null) {
				return;
			}

			ResetInfoViewFrame ();

			WhatNewLabel.StringValue = "What's New";
			CurrentDayLabel.StringValue = BookInfo.LastUpdatedDate.Value.ToString("dd MMM yyyy");
			UpdateInfoLabel.Hidden = true;

			List<GuideCard> addedGuideCardList = BookInfo.AddedGuideCard;
			List<GuideCard> delGuideCardList = BookInfo.DeletedGuideCard;
			List<GuideCard> updateGuideCardList = BookInfo.UpdatedGuideCard;

			nfloat addHeight = HeightCardList(addedGuideCardList);
			nfloat delHeight = HeightCardList(delGuideCardList);
			nfloat updateHeight = HeightCardList(updateGuideCardList);
			nfloat height = CELL_VERTICAL_SPACEING+addHeight+CELL_VERTICAL_SPACEING+delHeight+CELL_VERTICAL_SPACEING+updateHeight;

			whatsNewViewMinHeight = WhatNewView.Frame.Height;
			whatsNewViewMaxHeight = WhatNewView.Frame.Height-WhatNewInfoView.Frame.Bottom+ height+VERTICAL_SPACING;
			whatsNewInfoHeight = height;

			//fontsize:13:height:17 fontsize:12 height:16  space:3
			//6line 20*6+9
			nfloat curPointT = CurrentDayLabel.Frame.Top-18;
			CGRect frame = WhatNewInfoView.Frame;
			whatsNewInfoOrgPoint = new CGPoint (frame.Location.X, curPointT - whatsNewInfoHeight);

			if (height > 133) {
				isWhatsNewExpand = false;

				var nSize = new CGSize (frame.Size.Width, whatsNewInfoHeight);
				WhatNewInfoView.SetFrameSize (nSize);

				NSButton moreButton = CreateMoreButton ();
				WhatNewView.WantsLayer = true;
				moreButton.Action = new Selector ("WhatNewViewShowAll:");
				moreButton.Target = this;
				WhatNewView.AddSubview (moreButton);

			} else {

				if (frame.Size.Height < whatsNewInfoHeight) {
					var nSize = new CGSize (frame.Size.Width, whatsNewInfoHeight);
					WhatNewInfoView.SetFrameSize (nSize);
				}
			}

			nfloat curOrgy = WhatNewInfoView.Frame.Bottom-CELL_VERTICAL_SPACEING;
			curOrgy = AddGuidCardsToCustomView (addedGuideCardList, GuideCardMode.Added, curOrgy);

			curOrgy -= CELL_VERTICAL_SPACEING;
			curOrgy = AddGuidCardsToCustomView (delGuideCardList, GuideCardMode.Deleted, curOrgy);

			curOrgy -= CELL_VERTICAL_SPACEING;
			curOrgy = AddGuidCardsToCustomView (updateGuideCardList, GuideCardMode.Updated, curOrgy);

			WhatNewInfoView.SetFrameOrigin (whatsNewInfoOrgPoint);
		}
		#endif

		void ResetInfoViewFrame () 
		{
			var subviews = WhatNewInfoView.Subviews;
			nint infoCount = subviews.Length;
			nint index = 0;
			foreach (NSView view in subviews) {
				index++;
				if (index == 1) {
					continue;
				}

				view.RemoveFromSuperview ();
			}

			var views = WhatNewView.Subviews;
			var count = views.Length;
			var button = views [count - 1];
			if (button.Class.Name == "NSButton") {
				button.RemoveFromSuperview ();
			}

			if (infoCount > 1) {
				
				var oldSize = WhatNewView.Frame.Size;
				var newFrameSize = new CGSize (oldSize.Width, whatsNewViewMinHeight);
				WhatNewView.SetFrameSize (newFrameSize);

				CGRect frame = WhatNewInfoView.Frame;
				CGSize newSize = new CGSize (frame.Width, 114);
				CGPoint newPoint = new CGPoint (0, 5);

				WhatNewInfoView.SetFrameSize (newSize);
				WhatNewInfoView.SetFrameOrigin (newPoint);
			}
		}

		void InitializeInfomationView ()
		{
			if (BookInfo == null) {
				return;
			}

			//InformationView.Hidden = true;
			if (BookInfo.CurrentVersion>=0) {
				Version.StringValue = BookInfo.LastDownloadedVersion.ToString ();
			}
			if (BookInfo.InstalledDate != null) {
				InstalledDate.StringValue = BookInfo.InstalledDate.Value.ToString ("dd MMM yyyy");
			}
			if (BookInfo.CurrencyDate != null) {
				CurrencyDate.StringValue = BookInfo.CurrencyDate.Value.ToString ("dd MMM yyyy");
			}

			double mbSize = ((double)BookInfo.Size) / 1024 / 1024;
			string mbSizeStr = mbSize.ToString ("0.00");
			int count = mbSizeStr.LastIndexOf (".");
			string bookSize = mbSizeStr.Substring(0, count + 3)+ " MB";

			BookSize.StringValue = bookSize;
			if (BookInfo.PracticeArea != null) {
				PracticeArea.StringValue = BookInfo.PracticeArea;
			}
			if (BookInfo.SubCategory != null) {
				Subcategory.StringValue = BookInfo.SubCategory;
			}
		}


		//
		NSButton CreateMoreButton ()
		{
			var buttonFrame = new CGRect (370,-5,60,20);
			var moreButton = new NSButton(buttonFrame);
			moreButton.Bordered = false;
			moreButton.SetButtonType (NSButtonType.MomentaryChange);
			moreButton.AutoresizingMask = NSViewResizingMask.MinXMargin | NSViewResizingMask.MaxYMargin;
			moreButton.Cell.Alignment = NSTextAlignment.Center;
			NSAttributedString title = Utility.AttributeTitle("...more", Utility.ColorWithRGB(44,117,252), 12);
			moreButton.Cell.AttributedTitle = title;
			NSAttributedString alttitle = Utility.AttributeTitle ("...more", Utility.ColorWithRGB (77, 151, 250), 12);
			moreButton.Cell.AttributedAlternateTitle = alttitle;
			return moreButton;
		}

		#region calculate what's new view size
		nfloat HeightCardList (List<GuideCard> cardList)
		{
			int count = cardList == null ? 0 : cardList.Count;
			nfloat frameHeight = 0;
			nfloat cellHeight = 0;
			if (count != 0) {
				string titleName = "GuideCards Added:";
				cellHeight = Utility.HeightWrappedToWidth (titleName, 13, UpdateInfoLabel.Frame.Width);
				frameHeight += cellHeight;
				frameHeight += CELL_VERTICAL_SPACEING;

				foreach (var guideCard in cardList) {				
					string name = guideCard.Name;
					string comment = guideCard.Comments;
					if (!string.IsNullOrEmpty (name)) {
						cellHeight = Utility.HeightWrappedToWidth (name, 12, UpdateInfoLabel.Frame.Width);
						frameHeight += cellHeight+CELL_VERTICAL_SPACEING;
					}
				}
			} else {
				string name = "GuideCards Added:";

				cellHeight = Utility.HeightWrappedToWidth (name, 13, UpdateInfoLabel.Frame.Width);
				frameHeight += cellHeight*2+CELL_VERTICAL_SPACEING*2;
			}

			return frameHeight;
		}

		nfloat AddGuidCardsToCustomView(List<GuideCard> guideCardList, GuideCardMode cardMode, nfloat startPoint)
		{
			nfloat orgy = startPoint;
			nfloat frameHeight = 0;
			nfloat cellHeight = 0;
			int count = guideCardList.Count;
			string titleName = string.Empty;
			string titleComment = string.Empty;
			switch (cardMode) {
			case GuideCardMode.Added:
				titleName = "GuideCards Added:";
				titleComment = "No Files Added";
				break;

			case GuideCardMode.Deleted:
				titleName = "GuideCards Deleted:";
				titleComment = "No Files Deleted";
				break;

			case GuideCardMode.Updated:
				titleName = "GuideCards Updated:";
				titleComment = "No Files Updated";
				break;
			}

			if (count != 0) {

				orgy = AddGuideCard (titleName, null, orgy, 13);

				frameHeight += cellHeight;
				foreach (var guideCard in guideCardList) {				
					string name = guideCard.Name;
					string comment = guideCard.Comments;

					orgy = AddGuideCard (name, comment, orgy, 12);
				}
			} else {
				orgy = AddGuideCard (titleName, titleComment, orgy, 13);
			}

			return orgy;
		}

		nfloat AddGuideCard(string name, string comment, nfloat orgy, nfloat fontSize)
		{
			nfloat pointy = orgy;

			if (!string.IsNullOrEmpty(name)) {
				nfloat cellHeight = Utility.HeightWrappedToWidth (name, fontSize, UpdateInfoLabel.Frame.Width);
				pointy = orgy-cellHeight;

				var rect = new CGRect (6, pointy, UpdateInfoLabel.Frame.Width, cellHeight);
				var nameTF = new NSTextField (rect);
				nameTF.Cell.Bordered = false;
				nameTF.Cell.Font = NSFont.SystemFontOfSize (fontSize);
				nameTF.Cell.TextColor = Utility.ColorWithHexColorValue ("#666666",0.85f);
				nameTF.Cell.BackgroundColor = NSColor.Clear;
				nameTF.Cell.Wraps = true;
				nameTF.StringValue = name;
				nameTF.AutoresizingMask = NSViewResizingMask.MaxYMargin | NSViewResizingMask.MinXMargin;
				nameTF.Cell.Editable = false;

				WhatNewInfoView.AddSubview (nameTF);
				pointy -= CELL_VERTICAL_SPACEING;
			}
				
			if (!string.IsNullOrEmpty(comment)) {
				nfloat height = Utility.HeightWrappedToWidth (comment, fontSize, UpdateInfoLabel.Frame.Width);
				pointy -= height;

				var rect = new CGRect (6,pointy,UpdateInfoLabel.Frame.Width,height);
				var commentTF = new NSTextField(rect);
				commentTF.Cell.Bordered = false;
				commentTF.Cell.BackgroundColor = NSColor.Clear;
				commentTF.Cell.Font = NSFont.SystemFontOfSize (fontSize);
				commentTF.Cell.TextColor = Utility.ColorWithRGB (102, 102, 102);
				commentTF.StringValue = comment;
				commentTF.AutoresizingMask = NSViewResizingMask.MaxYMargin|NSViewResizingMask.MinXMargin;
				commentTF.Cell.Editable = false;
				WhatNewInfoView.AddSubview (commentTF);

				pointy -= CELL_VERTICAL_SPACEING;
			}

			return pointy;

		}
		#endregion

		nfloat HeightWrappedToWidth (NSTextField sourceTextField, float fontSize)
		{
			NSTextField textField = new NSTextField(new CGRect(0, 0, sourceTextField.Frame.Size.Width, 1000));
			textField.Font = NSFont.SystemFontOfSize(fontSize);
			textField.StringValue = sourceTextField.StringValue ;

			CGSize size = textField.Cell.CellSizeForBounds(textField.Frame);

			return size.Height;;
		}

		void LayoutSubViews ()
		{
			if (BookInfo == null) {
				return;
			}

			//calculate the subview size
			CGSize infoViewSize = InformationView.Frame.Size;
			CGSize whatsNewViewSize = WhatNewView.Frame.Size;

			CGSize casesViewSize;
			if (CaseView.Hidden) {
				casesViewSize = new CGSize (0,0);
			} else {
				casesViewSize = CaseView.Frame.Size;
			}

			CGSize despViewSize = DescriptionView.Frame.Size;

			CGSize expiredViewSize;
			if (BookInfo.DaysRemaining<0) {
				expiredViewSize = ExpiredView.Frame.Size;
			} else {
				expiredViewSize = new CGSize (0,0);
			}

			CGSize titleViewSize = TitleView.Frame.Size;

			//set scrollview contentview size
			nfloat caseSpace = casesViewSize.Height == 0 ? 0 : VERTICAL_SPACING;
			nfloat expirdSpace = expiredViewSize.Height == 0 ? 0 : VERTICAL_SPACING;
			nfloat height = VERTICAL_SPACING +
				infoViewSize.Height + VERTICAL_SPACING +
				whatsNewViewSize.Height + VERTICAL_SPACING +
				casesViewSize.Height + caseSpace +
				despViewSize.Height + VERTICAL_SPACING +
				expiredViewSize.Height + expirdSpace +
				titleViewSize.Height;

			CGRect scrollFrame = ScrollView.Frame;

			nfloat contentHeight = scrollFrame.Bottom > height ? scrollFrame.Bottom : height;

			var contentSize = new CGSize (AContentView.Frame.Size.Width, contentHeight);
			AContentView.SetFrameSize(contentSize);

			//add subview to document view
			var docView = (NSView)ScrollView.DocumentView;
			docView.SetFrameSize (contentSize);

			foreach (NSView view in docView.Subviews) {
				view.RemoveFromSuperview ();
			}

			docView.AddSubview(AContentView);
			AContentView.SetFrameOrigin(new CGPoint(0,0));

			//recalulate subviews origin point
			nfloat startOffset = scrollFrame.Bottom >= height ? (scrollFrame.Bottom-height+VERTICAL_SPACING) : VERTICAL_SPACING;
			nfloat yOffset;

			var InfoViewPoint = InformationView.Frame.Location;
			InfoViewPoint.Y = startOffset;
			InformationView.SetFrameOrigin (InfoViewPoint);

			var WhatsViewPoint = new CGPoint (InfoViewPoint.X,InfoViewPoint.Y+infoViewSize.Height+VERTICAL_SPACING);
			WhatNewView.SetFrameOrigin (WhatsViewPoint);

			var CaseViewPoint = new CGPoint (InfoViewPoint.X,WhatsViewPoint.Y+whatsNewViewSize.Height+VERTICAL_SPACING);
			CaseView.SetFrameOrigin (CaseViewPoint);

			if (casesViewSize.Height > 0) {
				yOffset = VERTICAL_SPACING;
			}else {
				yOffset = 0;
			}

			var DesViewPoint = new CGPoint (InfoViewPoint.X,CaseViewPoint.Y+casesViewSize.Height+yOffset);
			DescriptionView.SetFrameOrigin (DesViewPoint);

			var ExpireViewPoint = new CGPoint (InfoViewPoint.X,DesViewPoint.Y+despViewSize.Height+VERTICAL_SPACING);
			ExpiredView.SetFrameOrigin (ExpireViewPoint);

			if (expiredViewSize.Height > 0) {
				yOffset = VERTICAL_SPACING;
			}else {
				yOffset = 0;
			}

			var TitleViewPoint = new CGPoint (InfoViewPoint.X,ExpireViewPoint.Y+expiredViewSize.Height+yOffset);
			TitleView.SetFrameOrigin (TitleViewPoint);

			ScrollToTop ();
		}

		void ScrollToTop ()
		{
			CGPoint newScrollOrigin;

			// assume that the scrollview is an existing variable
			var documentView = (NSView)ScrollView.DocumentView;
			if (documentView.IsFlipped) {
				newScrollOrigin = new CGPoint(0.0f, 0.0f);
			} else {
				newScrollOrigin = new CGPoint(0.0f, documentView.Frame.Height
					-ScrollView.ContentView.Bounds.Height);
			}

			documentView.ScrollPoint(newScrollOrigin);
		}

		#endregion
	
		#region update
		partial void UpdatePublication (NSObject sender)
		{
			if (UpdateButton.Title == "Update") {
				UpdateButton.Title = "Cancel";
				ProgressView.Hidden = false;
				ProgressBar.MinValue = 0;
				ProgressBar.MaxValue = 1.0;
				ProgressStatusLabel.StringValue = "Downloading";
				StartDownloadWithoutLimitation(BookInfo, true);
			} else if (UpdateButton.Title == "Open") {
				IsOpenContentPanel = true;
				Window.PerformClose(sender);
			}
			else {
				string title = "Cancel Updates";
				string errMsg = "Are you sure you want to cancel the updates?";
				nint result = AlertSheet.RunConfirmAlert (title,errMsg);
				if (result == 1) {
					UpdateButton.Title = "Update";
					CancelUpdate(UpdateButton);
				}
			}
		}
			
		async void StartDownloadWithoutLimitation(Publication publication, bool isUpdate)
		{
			ProgressStatusLabel.StringValue = "Uploading...";
			tokenSource = new CancellationTokenSource ();

			DownloadResult downloadResult = await PublicationUtil.Instance.DownloadPublicationByBookId (publication.BookId, tokenSource.Token, UpdateDownloadProgress);

			string status = String.Empty;
			switch (downloadResult.DownloadStatus) {
			case DownLoadEnum.Canceled:
				status = "Update failed";
				SetProgressViewStatus (status);
				break;

			case DownLoadEnum.Failure:
				status = "Update failed";
				SetProgressViewStatus (status);

				break;

			case DownLoadEnum.NetDisconnected:
				status = "Missing connection";
				SetProgressViewStatus (status);

				break;

			case DownLoadEnum.Success:
				BookInfo = downloadResult.Publication;
				PublicationsDataManager.SharedInstance.ReplacePublicationByBookID (BookInfo); //update Publication data
				PublicationsDataManager.SharedInstance.CurrentPubliction = BookInfo;
				//Console.WriteLine ("DownLoadEnum.Success CurrentPubliction:update{0}", BookInfo.UpdateCount);
				SetProgressViewStatus (status);

				RefreshInfoView ();
				break;
			}

			AlertSheet.DestroyConfirmAlert ();
		}

		void SetProgressViewStatus(string status)
		{
			if (string.IsNullOrEmpty(status)) {
				ProgressView.Hidden = true;
				UpdateButton.Title = "Open";

			} else {
				
				NSAttributedString attributeTitle = Utility.AttributedTitle (status, 
					Utility.ColorWithHexColorValue ("#ff0000",1.0f), "System", 12.0f, NSTextAlignment.Left);
				ProgressStatusLabel.AttributedStringValue = attributeTitle;

				ProgressBar.DoubleValue = 0;
				UpdateButton.Title = "Update";
			}
		}

		void UpdateDownloadProgress(int bytesDownloaded, long downloadSize)
		{
			float curProgress = (float)(bytesDownloaded/100.0);

			ProgressBar.DoubleValue = curProgress;

			if ((bytesDownloaded - bytesPreDownloaded) > 2) {
				bytesPreDownloaded = bytesDownloaded;

				NSUserDefaults userDefault = NSUserDefaults.StandardUserDefaults;
				userDefault.SetInt (bytesDownloaded, BookInfo.Name);
				userDefault.Synchronize ();
			}

			if ( bytesDownloaded == 100) {
				NSUserDefaults userDefault = NSUserDefaults.StandardUserDefaults;
				userDefault.RemoveObject (BookInfo.Name);
				userDefault.Synchronize ();
			}
		}

		partial void CancelUpdate (NSObject sender)
		{
			if (tokenSource != null) {
				tokenSource.Cancel ();
			}
			RemovePublicationDownloadingProgress ();
		}

		private void RemovePublicationDownloadingProgress()
		{
			if (BookInfo.Name == null) {
				return;
			}

			NSUserDefaults userDefault = NSUserDefaults.StandardUserDefaults;
			userDefault.RemoveObject (BookInfo.Name);
			userDefault.Synchronize ();
		}

		private void RefreshInfoView()
		{
			//
			InitailizeTitleInfo ();

			//
			InitalizeExpiredView();

			//description view
			InitializeDescriptionView ();

			//case view
			InitializeCasesView ();

			//What new view
			InitializeWhatNewView ();

			//Information
			InitializeInfomationView ();

			LayoutSubViews ();

		}
		#endregion
	}
}

