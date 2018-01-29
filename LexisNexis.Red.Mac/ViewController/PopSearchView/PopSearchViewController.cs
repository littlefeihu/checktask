using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using CoreGraphics;
using LexisNexis.Red.Common.Entity;

namespace LexisNexis.Red.Mac
{
	public partial class PopSearchViewController : AppKit.NSViewController
	{
		#region properties
		NSPopover parentPopover;
		public SearchResultManager DataManager { get; set; }
		int BookID { get; set; }
		string KeyWord { get; set; }
		#endregion

		#region Constructors

		// Called when created from unmanaged code
		public PopSearchViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public PopSearchViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public PopSearchViewController (NSPopover popover, int bookID, string keyword) : base ("PopSearchView", NSBundle.MainBundle)
		{
			parentPopover = popover;
			BookID = bookID;
			KeyWord = keyword;

			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new PopSearchView View {
			get {
				return (PopSearchView)base.View;
			}
		}

		public override void AwakeFromNib ()
		{
			ResultLabel.StringValue = "Filter results by";

			DataManager = new SearchResultManager(this, BookID, KeyWord);

			InitializeOutlineView ();
		}

		public void InitializeOutlineView () 
		{
			if (DataManager == null || DataManager.PublicationResultList == null || 
				DataManager.PublicationResultList.Count == 0) {
				NoResultLabel.Hidden = false;
				NoResultLabel.StringValue = "You have no search result.";
				SearchOutlineView.Hidden = true;
			} else {
				NoResultLabel.Hidden = true;

				//IndexOutlineView.SelectionShouldChange += (t) => true;
				//IndexOutlineView.EnclosingScrollView.BackgroundColor = NSColor.Clear;

				SearchOutlineView.EnclosingScrollView.BorderType = NSBorderType.NoBorder;
				SearchOutlineView.EnclosingScrollView.BackgroundColor = NSColor.White;
				SearchOutlineView.EnclosingScrollView.HasHorizontalScroller = false;
				SearchOutlineView.EnclosingScrollView.HasVerticalScroller = true;
				SearchOutlineView.EnclosingScrollView.ScrollerKnobStyle = NSScrollerKnobStyle.Default;
				SearchOutlineView.EnclosingScrollView.VerticalScroller.ControlSize = NSControlSize.Small;
				SearchOutlineView.GridStyleMask = NSTableViewGridStyle.None;
				SearchOutlineView.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.Regular;
				SearchOutlineView.FloatsGroupRows = true;

				SearchOutlineView.DataSource = new SearchOutlineDataSource (this);
				SearchOutlineView.Delegate = new SearchOutlineDelegate (this);

				NSAnimationContext.BeginGrouping();
				NSAnimationContext.CurrentContext.Duration = 0;
				SearchOutlineView.ExpandItem (null, true);
				NSAnimationContext.EndGrouping();
			}
		}

		partial void PopupButtonSelectChange (NSObject sender)
		{
			var button = (NSPopUpButton)sender;

			nint index = button.IndexOfSelectedItem;
			var rect = button.Frame;
			CGSize newSize;
			CGPoint newPoint = rect.Location;
			switch (index) {
			case 1:
				newSize = new CGSize(42, rect.Height);
				button.SetFrameSize(newSize);
				newPoint.X = 248;
				button.SetFrameOrigin(newPoint);
				break;
			case 2:
				newSize = new CGSize(100, rect.Height);
				button.SetFrameSize(newSize);
				newPoint.X = 190;
				button.SetFrameOrigin(newPoint);
				break;
			case 3:
				newSize = new CGSize(110, rect.Height);
				button.SetFrameSize(newSize);
				newPoint.X = 180;
				button.SetFrameOrigin(newPoint);
				break;
			case 4:
				newSize = new CGSize(158, rect.Height);
				button.SetFrameSize(newSize);
				newPoint.X = 132;
				button.SetFrameOrigin(newPoint);
				break;
			}

			button.Title = button.TitleOfSelectedItem;
		}

		#region api with IndexDataManager
		public void RefreshSearchViewData ()
		{
			if (SearchOutlineView != null) {
				this.InvokeOnMainThread(()=>SearchOutlineView.ReloadData ());
			}
		}

		//api with IndexViewController 
		public void OpenPublicationAtTocNode (SearchDisplayResult tocNode)
		{
			parentPopover.Close ();
		}
		#endregion
	}
}
