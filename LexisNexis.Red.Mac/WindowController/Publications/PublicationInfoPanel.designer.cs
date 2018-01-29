// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace LexisNexis.Red.Mac
{
	[Register ("PublicationInfoPanelController")]
	partial class PublicationInfoPanelController
	{
		[Outlet]
		AppKit.NSView AContentView { get; set; }

		[Outlet]
		AppKit.NSTextField AreaLabel { get; set; }

		[Outlet]
		AppKit.NSTextField AuthorLabel { get; set; }

		[Outlet]
		AppKit.NSTextField BookSize { get; set; }

		[Outlet]
		AppKit.NSTextField BookSizeLabel { get; set; }

		[Outlet]
		AppKit.NSTextField BookTitleLabel { get; set; }

		[Outlet]
		AppKit.NSTextField CaseLabel { get; set; }

		[Outlet]
		AppKit.NSButton CasesButton { get; set; }

		[Outlet]
		AppKit.NSTextField CaseTextField { get; set; }

		[Outlet]
		AppKit.NSView CaseView { get; set; }

		[Outlet]
		AppKit.NSButton ContactEmailLabel { get; set; }

		[Outlet]
		AppKit.NSTextField ContactPhoneLabel { get; set; }

		[Outlet]
		AppKit.NSView CoverImageView { get; set; }

		[Outlet]
		AppKit.NSTextField CurrencyDate { get; set; }

		[Outlet]
		AppKit.NSTextField CurrencyDateLabel { get; set; }

		[Outlet]
		AppKit.NSTextField CurrentDayLabel { get; set; }

		[Outlet]
		AppKit.NSTextField DescriptionLabel { get; set; }

		[Outlet]
		AppKit.NSView DescriptionView { get; set; }

		[Outlet]
		AppKit.NSTextField DespTextField { get; set; }

		[Outlet]
		AppKit.NSButton DotButton { get; set; }

		[Outlet]
		AppKit.NSTextField ExpiredInfoTF { get; set; }

		[Outlet]
		AppKit.NSTextField ExpiredLabel { get; set; }

		[Outlet]
		AppKit.NSView ExpiredView { get; set; }

		[Outlet]
		AppKit.NSTextField InfoLabel { get; set; }

		[Outlet]
		AppKit.NSView InformationView { get; set; }

		[Outlet]
		AppKit.NSTextField InstalledDate { get; set; }

		[Outlet]
		AppKit.NSTextField InstalledLabel { get; set; }

		[Outlet]
		AppKit.NSButton LoanButton { get; set; }

		[Outlet]
		AppKit.NSTextField PracticeArea { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator ProgressBar { get; set; }

		[Outlet]
		AppKit.NSTextField ProgressStatusLabel { get; set; }

		[Outlet]
		AppKit.NSView ProgressView { get; set; }

		[Outlet]
		AppKit.NSScrollView ScrollView { get; set; }

		[Outlet]
		AppKit.NSTextField Subcategory { get; set; }

		[Outlet]
		AppKit.NSTextField SubcategoryLabel { get; set; }

		[Outlet]
		AppKit.NSView TagsView { get; set; }

		[Outlet]
		AppKit.NSView TitleView { get; set; }

		[Outlet]
		AppKit.NSButton UpdateButton { get; set; }

		[Outlet]
		AppKit.NSTextField UpdateInfoLabel { get; set; }

		[Outlet]
		AppKit.NSTextField Version { get; set; }

		[Outlet]
		AppKit.NSTextField VersionLabel { get; set; }

		[Outlet]
		AppKit.NSTextField WhatNewInfoLabel { get; set; }

		[Outlet]
		AppKit.NSView WhatNewInfoView { get; set; }

		[Outlet]
		AppKit.NSTextField WhatNewLabel { get; set; }

		[Outlet]
		AppKit.NSView WhatNewView { get; set; }

		[Action ("ContactEmail:")]
		partial void ContactEmail (Foundation.NSObject sender);

		[Action ("UpdatePublication:")]
		partial void UpdatePublication (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AContentView != null) {
				AContentView.Dispose ();
				AContentView = null;
			}

			if (AreaLabel != null) {
				AreaLabel.Dispose ();
				AreaLabel = null;
			}

			if (AuthorLabel != null) {
				AuthorLabel.Dispose ();
				AuthorLabel = null;
			}

			if (BookSize != null) {
				BookSize.Dispose ();
				BookSize = null;
			}

			if (BookSizeLabel != null) {
				BookSizeLabel.Dispose ();
				BookSizeLabel = null;
			}

			if (BookTitleLabel != null) {
				BookTitleLabel.Dispose ();
				BookTitleLabel = null;
			}

			if (CaseLabel != null) {
				CaseLabel.Dispose ();
				CaseLabel = null;
			}

			if (CasesButton != null) {
				CasesButton.Dispose ();
				CasesButton = null;
			}

			if (CaseTextField != null) {
				CaseTextField.Dispose ();
				CaseTextField = null;
			}

			if (CaseView != null) {
				CaseView.Dispose ();
				CaseView = null;
			}

			if (ContactEmailLabel != null) {
				ContactEmailLabel.Dispose ();
				ContactEmailLabel = null;
			}

			if (ContactPhoneLabel != null) {
				ContactPhoneLabel.Dispose ();
				ContactPhoneLabel = null;
			}

			if (CoverImageView != null) {
				CoverImageView.Dispose ();
				CoverImageView = null;
			}

			if (CurrencyDate != null) {
				CurrencyDate.Dispose ();
				CurrencyDate = null;
			}

			if (CurrencyDateLabel != null) {
				CurrencyDateLabel.Dispose ();
				CurrencyDateLabel = null;
			}

			if (CurrentDayLabel != null) {
				CurrentDayLabel.Dispose ();
				CurrentDayLabel = null;
			}

			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}

			if (DescriptionView != null) {
				DescriptionView.Dispose ();
				DescriptionView = null;
			}

			if (DespTextField != null) {
				DespTextField.Dispose ();
				DespTextField = null;
			}

			if (ExpiredInfoTF != null) {
				ExpiredInfoTF.Dispose ();
				ExpiredInfoTF = null;
			}

			if (ExpiredLabel != null) {
				ExpiredLabel.Dispose ();
				ExpiredLabel = null;
			}

			if (ExpiredView != null) {
				ExpiredView.Dispose ();
				ExpiredView = null;
			}

			if (InfoLabel != null) {
				InfoLabel.Dispose ();
				InfoLabel = null;
			}

			if (InformationView != null) {
				InformationView.Dispose ();
				InformationView = null;
			}

			if (InstalledDate != null) {
				InstalledDate.Dispose ();
				InstalledDate = null;
			}

			if (InstalledLabel != null) {
				InstalledLabel.Dispose ();
				InstalledLabel = null;
			}

			if (LoanButton != null) {
				LoanButton.Dispose ();
				LoanButton = null;
			}

			if (PracticeArea != null) {
				PracticeArea.Dispose ();
				PracticeArea = null;
			}

			if (ProgressBar != null) {
				ProgressBar.Dispose ();
				ProgressBar = null;
			}

			if (ProgressStatusLabel != null) {
				ProgressStatusLabel.Dispose ();
				ProgressStatusLabel = null;
			}

			if (ProgressView != null) {
				ProgressView.Dispose ();
				ProgressView = null;
			}

			if (ScrollView != null) {
				ScrollView.Dispose ();
				ScrollView = null;
			}

			if (Subcategory != null) {
				Subcategory.Dispose ();
				Subcategory = null;
			}

			if (SubcategoryLabel != null) {
				SubcategoryLabel.Dispose ();
				SubcategoryLabel = null;
			}

			if (TagsView != null) {
				TagsView.Dispose ();
				TagsView = null;
			}

			if (TitleView != null) {
				TitleView.Dispose ();
				TitleView = null;
			}

			if (UpdateButton != null) {
				UpdateButton.Dispose ();
				UpdateButton = null;
			}

			if (UpdateInfoLabel != null) {
				UpdateInfoLabel.Dispose ();
				UpdateInfoLabel = null;
			}

			if (Version != null) {
				Version.Dispose ();
				Version = null;
			}

			if (VersionLabel != null) {
				VersionLabel.Dispose ();
				VersionLabel = null;
			}

			if (WhatNewInfoLabel != null) {
				WhatNewInfoLabel.Dispose ();
				WhatNewInfoLabel = null;
			}

			if (WhatNewInfoView != null) {
				WhatNewInfoView.Dispose ();
				WhatNewInfoView = null;
			}

			if (WhatNewLabel != null) {
				WhatNewLabel.Dispose ();
				WhatNewLabel = null;
			}

			if (WhatNewView != null) {
				WhatNewView.Dispose ();
				WhatNewView = null;
			}

			if (DotButton != null) {
				DotButton.Dispose ();
				DotButton = null;
			}
		}
	}

	[Register ("PublicationInfoPanel")]
	partial class PublicationInfoPanel
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
