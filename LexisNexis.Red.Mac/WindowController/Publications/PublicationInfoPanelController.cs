
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
using LexisNexis.Red.Common.HelpClass;

namespace LexisNexis.Red.Mac
{
	public partial class PublicationInfoPanelController : NSWindowController
	{
		#region Properties
		public NSImage bookCoverImage {get; set;}
		public Publication BookInfo { get; set;}

		public bool IsOpenContentPanel {get; set;}

		bool isFTC;
		//CircularProgressView progressView;

		static nfloat expiredViewMinHeight;
		static nfloat expiredViewMaxHeight;
		bool isExpiredExpand { get; set;}

		static nfloat descriptionViewHeight;
		bool isDescripExpand { get; set; }

		static nfloat whatnewViewHeight;
		bool isWhatsNewExpand { get; set; }

		static nfloat caseViewHeight;
		bool isCaseViewExpand { get; set;}

		public enum DownloadStatusEnum {
			None = 0,
			Downloading = 1,
			Finished = 2,
			Cancel = 3
		}
				
		public int UpdateStatus { get; set;} //0:None 1:Downloading 2:Finished 3:Cancel

		const float VERTICAL_SPACING = 5;
		const float CELL_VERTICAL_SPACEING = 3;
		const float GUIDE_TEXTFIELD_HEIGHT = 18;
		const float TEXTVIEW_SIXLINE_HEIGHT = 104;

		//strongly typed window accessor
		public new PublicationInfoPanel Window {
			get {
				return (PublicationInfoPanel)base.Window;
			}
		}

		public PublicationView BookViewer;
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
		public PublicationInfoPanelController (CGPoint location, PublicationView aViewer) : base ("PublicationInfoPanel")
		{
			Window.AnimationBehavior = NSWindowAnimationBehavior.Default;
			Window.SetFrameOrigin (location);
			BookViewer = aViewer;
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
			Window.Title = "Book Details";

			Window.BackgroundColor = NSColor.White;
			Window.MakeFirstResponder (null);

			ScrollView.ScrollerKnobStyle = NSScrollerKnobStyle.Default;
			ScrollView.VerticalScroller.ControlSize = NSControlSize.Small;
		}

		#endregion


		#region methods
		public void InitializeInfoView(int status)
		{
			isFTC = BookInfo.IsFTC;
			UpdateStatus = status;
			//
			InitializeCoverView ();

			RefreshInfoView ();
		}
			
		//Cover view
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

			if (UpdateStatus == 1) {
				ShowUpdateProgressView ();
			} else {
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
		}

		void ShowUpdateProgressView()
		{
			UpdateButton.Title = "Cancel";
			ProgressView.Hidden = false;
			ProgressBar.MinValue = 0;
			ProgressBar.MaxValue = 1.0;
			ProgressStatusLabel.StringValue = "Downloading";
		}

		//title view
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
			button.Layer.BackgroundColor = Utility.ColorWithRGB (132, 132, 132, 1.0f).CGColor; //132
			button.Layer.CornerRadius = 8.0f;
			button.Cell.AttributedTitle = Utility.AttributeTitle (title, 
				Utility.ColorWithHexColorValue ("#ececec", 1.0f), 12);  //236
			button.Cell.AttributedAlternateTitle = Utility.AttributeTitle ("Loan", 
				Utility.ColorWithHexColorValue ("#ececec", 1.0f), 12);
		}

		//expire view
		void InitalizeExpiredView()
		{
			LoginUserDetails userDetail = GlobalAccess.Instance.CurrentUserInfo;
			string email = userDetail.Country.CustomerSupportEmail;
			string telephone = userDetail.Country.CustomerSupportTEL;

			string contactString =  "Contact " + telephone + " or " + email;

			ContactPhoneLabel.AllowsEditingTextAttributes = true;
			ContactPhoneLabel.AttributedStringValue = Utility.AttributeLinkTitle(contactString,email,"Expired", "Support",
				Utility.ColorWithHexColorValue ("#000000",0.85f), 14);

//			ContactEmailLabel.AttributedTitle = Utility.AttributeTitle (email,
//				Utility.ColorWithHexColorValue ("#0080fc",0.85f), 14);
//			ContactEmailLabel.AttributedAlternateTitle = Utility.AttributeTitle (email,
//				Utility.ColorWithHexColorValue ("#0080fc",0.45f), 14);

			if (BookInfo == null) {
				return;
			}

			if (BookInfo.DaysRemaining >= 0) {
				ExpiredView.Hidden = true;
			} else {
				ExpiredView.Hidden = false;
			}

			DotButton.Cell.BezelStyle = NSBezelStyle.Circular;
			DotButton.Cell.Bordered = false;
			DotButton.WantsLayer = true;
			DotButton.Layer.BackgroundColor = Utility.ColorWithHexColorValue ("#ed1c24",0.85f).CGColor;
			DotButton.Layer.CornerRadius = 3.5f;

			float fontSize = 13.0f;
			string description = string.Format ("Your subscription to this title has expired.\n" +
				"We want to thank you for choosing LexisNexis to support the great work you do on a daily basis, " +
				"and we hope to continue being your legal content provider of choice.\n" +
				"To re-subscribe to this content and continue receiving updates, " +
				"please contact your Relationship Manager, or " +
				"Customer Service on {0} or {1}\n" +
				"Should you choose not to resubscribe to this title, there are a few things we want to make sure you're aware of.\n" +
				"    ·   This content will no longer receive updates from LexisNexis, \n" +
				"        and will be out of date as of the next update.\n" +
				"    ·   Annotations made after the expiry of this title will no longer \n" +
				"        sync. And could be lost upon subscribing.\n" +
				"    ·   If you delete this book from, or lose your device, LexisNexis \n" +
				"        is unable to retrieve the content for you.\n" +
				"Should you choose to resubscribe to this content, full functionality will be restored, " +
				"and content automatically updated to the most recent version.\n" +
				"Please don't hesitate to contact your Relationship Manager should you have any questions or concerns regarding this, " +
				"or any of your other Red titles.",telephone,email);

			ExpiredInfoTF.AllowsEditingTextAttributes = true;
			ExpiredInfoTF.AttributedStringValue = Utility.AttributeLinkTitle (description, 
				email, "Expired", "Support", Utility.ColorWithHexColorValue ("#666666",0.85f), fontSize);

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

		public override void MouseUp (NSEvent theEvent)
		{
			//base.MouseUp (theEvent);
			//Console.WriteLine("MouseUp");
		}

		[Action("ExpiredViewShowAll:")]
		void ExpiredViewShowAll(NSObject sender)
		{
			float fontSize = 13.0f;
			if (!isExpiredExpand) {
				var oldSize = ExpiredView.Frame.Size;
				var newSize = new CGSize (oldSize.Width, expiredViewMaxHeight);
				ExpiredView.SetFrameSize (newSize);
				var subViews = ExpiredView.Subviews;
				var button = (NSButton)subViews [subViews.Length-1]; //对照xib 确定id
				NSAttributedString title = Utility.AttributeTitle("...less", Utility.ColorWithRGB(44,117,252,1.0f), fontSize);
				button.Cell.AttributedTitle = title;

				isExpiredExpand = true;

			} else {
				var oldSize = ExpiredView.Frame.Size;
				var newSize = new CGSize (oldSize.Width, expiredViewMinHeight);
				ExpiredView.SetFrameSize (newSize);

				var subViews = ExpiredView.Subviews;
				var button = (NSButton)subViews [subViews.Length-1]; 

				NSAttributedString title = Utility.AttributeTitle("...more", Utility.ColorWithRGB(44,117,252,1.0f), fontSize);
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

		//cases
		void InitializeCasesView ()
		{
			if (isFTC) {
				CaseView.Hidden = false;
			} else {
				CaseView.Hidden = true;
			}

			if (BookInfo == null) {
				return;
			}

			float fontSize = 13.0f;

			string description = "What can you expect from your subscription to the new + Cases titles: \n" +
				"- The same great content you know and trust, automatically updated. \n" +
				"- Inclusion of new additional content from our unreported judgment \n" +
				"  database: \n" +
				"   - High value text cases chosen based on their high degree of \n" +
				"     relevance to this title; \n" +
				"   - Offline accessibility to this content within your Red title; \n" +
				"   - Ability to link between the analytical and case content within \n" +
				"     this title, and between your other Red titles; \n" +
				"   - Use of Red’s award winning annotation, highlighting and legal \n" +
				"     define features on the text cases within this title. \n" +
				"\n" +
				"Please don’t hesitate to contact your Relationship Manager should you have any questions or feedback regarding your subscription. ";

			CaseTextField.StringValue = description;

			caseViewHeight = CaseView.Frame.Size.Height;

			var height = HeightWrappedToWidth (CaseTextField,fontSize);

			if (height > TEXTVIEW_SIXLINE_HEIGHT) {

				NSButton moreButton = CreateMoreButton ();
				DescriptionView.WantsLayer = true;
				moreButton.Action = new Selector ("CaseViewShowAll:");
				moreButton.Target = this;
				CaseView.AddSubview (moreButton);

				isCaseViewExpand = false;
			} else {
				nfloat orgHeight = CaseTextField.Frame.Size.Height;
				nfloat offset = height - orgHeight;
				var oldSize = CaseView.Frame.Size;
				var newSize = new CGSize (oldSize.Width, caseViewHeight+offset);
				CaseView.SetFrameSize (newSize);
			}
		}

		[Action("CaseViewShowAll:")]
		void CaseViewShowAll(NSObject sender)
		{
			float fontSize = 13.0f;
			nfloat height = HeightWrappedToWidth (CaseTextField,fontSize);

			if (!isCaseViewExpand) {
				nfloat orgHeight = CaseTextField.Frame.Size.Height;
				nfloat offset = height - orgHeight;
				var oldSize = DescriptionView.Frame.Size;
				var newSize = new CGSize (oldSize.Width, caseViewHeight+offset);
				CaseView.SetFrameSize (newSize);

				var button = (NSButton)CaseView.Subviews [3];
				NSAttributedString title = Utility.AttributeTitle("...less", Utility.ColorWithRGB(44,117,252,1.0f), 12);
				button.Cell.AttributedTitle = title;

				isCaseViewExpand = true;

			} else {
				var oldSize = CaseView.Frame.Size;
				var newSize = new CGSize (oldSize.Width, caseViewHeight);
				CaseView.SetFrameSize (newSize);

				var button = (NSButton)CaseView.Subviews [3];
				NSAttributedString title = Utility.AttributeTitle("...more", Utility.ColorWithRGB(44,117,252,1.0f), 12);
				button.Cell.AttributedTitle = title;

				isCaseViewExpand = false;
			}

			LayoutSubViews ();
		}
			
		//description view
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
				NSAttributedString title = Utility.AttributeTitle("...less", Utility.ColorWithRGB(44,117,252,1.0f), 12);
				button.Cell.AttributedTitle = title;

				isDescripExpand = true;

			} else {
				var oldSize = DescriptionView.Frame.Size;
				var newSize = new CGSize (oldSize.Width, descriptionViewHeight);
				DescriptionView.SetFrameSize (newSize);

				var button = (NSButton)DescriptionView.Subviews [3];
				NSAttributedString title = Utility.AttributeTitle("...more", Utility.ColorWithRGB(44,117,252,1.0f), 12);
				button.Cell.AttributedTitle = title;

				isDescripExpand = false;
			}

			LayoutSubViews ();
		}

		//what's new view 
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
				addString += "No Files Added\n";
			}

			string deleteString = "GuideCards Deleted:\n";
			if (delGuideCardList.Count > 0) {
				foreach (GuideCard guidCard in delGuideCardList) {
					if (!string.IsNullOrEmpty (guidCard.Name)) {
						deleteString += guidCard.Name + "\n";
					}
				}
			} else {
				deleteString += "No Files Deleted\n";
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
			whatnewViewHeight = WhatNewView.Frame.Size.Height;

			nfloat height = HeightOfWhatNewInfoView ();
			if (height > TEXTVIEW_SIXLINE_HEIGHT) {

				NSButton moreButton = CreateMoreButton ();
				WhatNewView.WantsLayer = true;
				moreButton.Action = new Selector ("WhatNewViewShowAll:");
				moreButton.Target = this;
				WhatNewView.AddSubview (moreButton);

				isWhatsNewExpand = false;
			} else {
				nfloat orgHeight = WhatNewInfoLabel.Frame.Size.Height;
				nfloat offset = height - orgHeight;
				var oldSize = WhatNewInfoLabel.Frame.Size;
				var newSize = new CGSize (oldSize.Width, whatnewViewHeight+offset);
				WhatNewView.SetFrameSize (newSize);
			}
		}

		[Action("WhatNewViewShowAll:")]
		void WhatNewViewShowAll(NSObject sender)
		{
			float fontSize = 13.0f;
			nfloat height = HeightWrappedToWidth (WhatNewInfoLabel,fontSize);

			if (!isWhatsNewExpand) {
				nfloat orgHeight = WhatNewInfoLabel.Frame.Size.Height;
				nfloat offset = height - orgHeight;
				var oldSize = WhatNewView.Frame.Size;
				var newSize = new CGSize (oldSize.Width, whatnewViewHeight+offset);
				WhatNewView.SetFrameSize (newSize);

				var subviews = WhatNewView.Subviews;
				int index = subviews.Length - 1;
				var button = (NSButton)subviews [index];
				NSAttributedString title = Utility.AttributeTitle("...less", Utility.ColorWithRGB(44,117,252,1.0f), 12);
				button.Cell.AttributedTitle = title;

				isWhatsNewExpand = true;

			} else {
				var oldSize = WhatNewView.Frame.Size;
				var newSize = new CGSize (oldSize.Width, whatnewViewHeight);
				WhatNewView.SetFrameSize (newSize);

				var subviews = WhatNewView.Subviews;
				int index = subviews.Length - 1;
				var button = (NSButton)subviews [index];

				NSAttributedString title = Utility.AttributeTitle("...more", Utility.ColorWithRGB(44,117,252,1.0f), 12);
				button.Cell.AttributedTitle = title;

				isWhatsNewExpand = false;
			}

			LayoutSubViews ();
		}

		//info view
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
			var buttonFrame = new CGRect (370,-4,60,20);
			var moreButton = new NSButton(buttonFrame);
			moreButton.Bordered = false;
			moreButton.SetButtonType (NSButtonType.MomentaryChange);
			moreButton.AutoresizingMask = NSViewResizingMask.MinXMargin | NSViewResizingMask.MaxYMargin;
			moreButton.Cell.Alignment = NSTextAlignment.Center;
			NSAttributedString title = Utility.AttributeTitle("...more", Utility.ColorWithRGB(44,117,252,1.0f), 12);
			moreButton.Cell.AttributedTitle = title;
			NSAttributedString alttitle = Utility.AttributeTitle ("...more", Utility.ColorWithRGB (77, 151,250,1.0f), 12);
			moreButton.Cell.AttributedAlternateTitle = alttitle;
			return moreButton;
		}
			
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
	

		#region update
		partial void UpdatePublication (NSObject sender)
		{
			if (UpdateButton.Title == "Update") {
				UpdateButton.Title = "Cancel";
				ProgressView.Hidden = false;
				ProgressBar.MinValue = 0;
				ProgressBar.MaxValue = 1.0;
				ProgressStatusLabel.StringValue = "Downloading";
				if (BookViewer != null) {
				    BookViewer.DownloadController.StartDownloadWithoutLimitation(BookInfo.BookId,true);
					UpdateStatus = 1;
				}
			} else if (UpdateButton.Title == "Open") {
				UpdateStatus = 0;
				IsOpenContentPanel = true;
				Window.PerformClose(sender);
			}
			else {
				string title = "Cancel Updates";
				string errorMsg = "Are you sure you want to cancel the updates?";
				nint result = AlertSheet.RunConfirmAlert (title,errorMsg);
				if (result == 1) {
					if (BookViewer != null) {
					    BookViewer.DownloadController.CancelDownload();
					}
					UpdateButton.Title = "Update";
					ProgressBar.DoubleValue = 0;
					UpdateStatus = 3;

				}else {
				}
			}
		}

		public void UpdateDownloadProgress(int bytesDownloaded, long downloadSize)
		{
			UpdateStatus = 1;

			float curProgress = (float)(bytesDownloaded/100.0);
			ProgressBar.DoubleValue = curProgress;
		}

		public void RefreshDownloadStatus (DownloadResult downloadResult, bool isUpdate)
		{
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
				PublicationsDataManager.SharedInstance.CurrentPublication = BookInfo;

				SetProgressViewStatus (status);
				UpdateStatus = 2;

				RefreshInfoView ();

				NSNotificationCenter.DefaultCenter.PostNotificationName(LNRConstants.LNPublicationDidFinishedDownload,
					NSObject.FromObject(BookInfo.BookId));

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

		#endregion
	}
}

