
using System;
using Foundation;
using AppKit;
using LexisNexis.Red.Mac.Data;
using CoreGraphics;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using System.Threading.Tasks;

namespace LexisNexis.Red.Mac
{
	public partial class TocViewController : NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public TocViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public TocViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public TocViewController () : base ("TocView", NSBundle.MainBundle)
		{
			Initialize ();
		}
			
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		#region properties 
		//strongly typed view accessor
		public new TocView View {
			get {
				return (TocView)base.View;
			}
		}

		public int BookID { get; set;}
		public bool IsExpired { get; set;}
		public string CurrencyDate { get; set;}
		public PublicationTOCDataManager TOCDataManager { get;set;}
		LoadingViewController loadViewCtr;

		#endregion

		#region methods
		public void InitializeTableView (bool isAddLoadView) 
		{
			if (IsExpired) {
				ShowExpiredInfoView ();
			} else {
				RemoveExpiredInfoView ();
			}

			if (isAddLoadView) {
				return;
			}

			AddLoadView ();
			TocTableView.AllowsColumnSelection = false;
			TocTableView.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.Regular;

			TocTableView.EnclosingScrollView.HasHorizontalScroller = false;
			TocTableView.AllowsColumnResizing = true;
			TocTableView.EnclosingScrollView.ScrollerKnobStyle = NSScrollerKnobStyle.Default;
			TocTableView.EnclosingScrollView.VerticalScroller.ControlSize = NSControlSize.Small;
			TocTableView.ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.Uniform;

			TocTableView.DataSource = new TocTableViewDataSources (this);
			TocTableView.Delegate = new TocTableViewDelegate (this);
			TocTableView.EnclosingScrollView.BorderType = NSBorderType.NoBorder;
			TocTableView.GridStyleMask = NSTableViewGridStyle.None;
		}

		private void AddLoadView ()
		{
			if (loadViewCtr == null) {

				var scrollframe = TocTableView.EnclosingScrollView.Frame;
				var location = new CGPoint ();
				location.X = (scrollframe.Width - 200) / 2;
				location.Y = (scrollframe.Height - 100) / 2;

				loadViewCtr = new LoadingViewController (location, LNRConstants.LOADING_INFO);
			} 
			TocTableView.Superview.AddSubview (loadViewCtr.View);
		}

		private void RemoveLoadView ()
		{
			if (loadViewCtr != null) {
				loadViewCtr.View.RemoveFromSuperview ();
				loadViewCtr.Dispose ();
				loadViewCtr = null;
			}
		}

		public void RefreshTableViewData()
		{
			RemoveLoadView ();
			TocTableView.ReloadData ();
		}

		public void ScrollCurrentLeafNodeToVisible ()
		{
			TocTableView.ScrollRowToVisible (TOCDataManager.CurrentHighlight);
		}

		public void SelectTOCRectByTocID(int tocNodeID)
		{
			if (TOCDataManager == null) {
				return;
			}

			int rowIndexHighLight = TOCDataManager.IndexByTOCID (tocNodeID);
			if (rowIndexHighLight == 0) {
				return;
			}

			TocTableView.SelectRow (rowIndexHighLight, false);
		}
			
		public async Task OpenPublicationContentAtTOCNode (TOCNode tocNode)
		{
			await TOCDataManager.PanelController.OpenPublicationContentAtTOCNode (BookID, tocNode);
		}

		private void ShowExpiredInfoView ()
		{
			foreach (var view in ExpiredInfoView.Subviews) {
				view.RemoveFromSuperview ();
			}

			nfloat viewHeight = LNRConstants.TOCITEMHEIGHT_MIN;
			nfloat tfHeight = 17;
			var scrollView = TocTableView.EnclosingScrollView;
			var superViewSize = scrollView.Superview.Frame.Size;
			nfloat scrollHeight = superViewSize.Height - viewHeight;
			var newSize = new CGSize (scrollView.Frame.Width, scrollHeight);
			scrollView.SetFrameSize (newSize);
			var newPoint = new CGPoint (0,0);
			scrollView.SetFrameOrigin (newPoint);

			var infoViewOrg = new CGPoint (0,newSize.Height);
			ExpiredInfoView.SetFrameOrigin (infoViewOrg);

			ExpiredInfoView.WantsLayer = true;
			ExpiredInfoView.Layer.BackgroundColor = NSColor.Red.CGColor;

			CGRect frame = ExpiredInfoView.Frame;
			var expRect = new CGRect (10, tfHeight+5, frame.Width-15, tfHeight);
			var expiredTF = new NSTextField (expRect);
			expiredTF.Cell.Bordered = false;
			expiredTF.Cell.Editable = false;
			expiredTF.Cell.DrawsBackground = false;
			var attributtedTitle = Utility.AttributedTitle ("Expired", NSColor.White, "System", 14,NSTextAlignment.Left);
			expiredTF.AttributedStringValue = attributtedTitle;
			expiredTF.AutoresizingMask = NSViewResizingMask.MinXMargin|NSViewResizingMask.MaxYMargin;
			ExpiredInfoView.AddSubview (expiredTF);

			var dateRect = new CGRect (10, 5, frame.Width-15, tfHeight);
			var dateTF = new NSTextField (dateRect);
			dateTF.Cell.Bordered = false;
			dateTF.Cell.Editable = false;
			dateTF.Cell.DrawsBackground = false;
			attributtedTitle = Utility.AttributedTitle (CurrencyDate, NSColor.Grid, "System", 12,NSTextAlignment.Left);
			dateTF.AttributedStringValue = attributtedTitle;
			dateTF.AutoresizingMask = NSViewResizingMask.MinXMargin|NSViewResizingMask.MaxYMargin;
			ExpiredInfoView.AddSubview (dateTF);

			var infoRect = new CGRect (frame.Width-60, 12, 50, 20);
			var infoButton = new NSButton (infoRect);
			infoButton.Cell.BezelStyle = NSBezelStyle.ShadowlessSquare;
			infoButton.Cell.AttributedTitle = Utility.AttributeTitle ("Info", NSColor.White, 12);
			infoButton.Cell.AttributedAlternateTitle = Utility.AttributeTitle ("Info", NSColor.Black, 12);
			infoButton.Cell.SetButtonType (NSButtonType.MomentaryPushIn);
			infoButton.Bordered = false;
			infoButton.ShowsBorderOnlyWhileMouseInside = true;
			infoButton.WantsLayer = true;
			infoButton.Layer.BorderColor = NSColor.White.CGColor;
			infoButton.Layer.BorderWidth = 1;
			infoButton.Layer.CornerRadius = 4;
			infoButton.Layer.BackgroundColor = NSColor.Red.CGColor;

			infoButton.Target = this;
			infoButton.Action = new ObjCRuntime.Selector ("TitleInfoClick:");
			ExpiredInfoView.AddSubview (infoButton);

			View.AddSubview (ExpiredInfoView);
		}

		private void RemoveExpiredInfoView ()
		{
			ExpiredInfoView.RemoveFromSuperview ();

			var scrollView = TocTableView.EnclosingScrollView;
			var scrollViewSize = scrollView.Frame.Size;
			var superViewSize = scrollView.Superview.Frame.Size;
			var newSize = new CGSize (scrollViewSize.Width, superViewSize.Height);
			scrollView.SetFrameSize (newSize);
			var newPoint = new CGPoint (0,0);
			scrollView.SetFrameOrigin (newPoint);
		}

		public override void MouseUp (NSEvent theEvent)
		{
			CGPoint location = theEvent.LocationInWindow;
			CGRect frame = ExpiredInfoView.Frame;
			bool isInFrame = frame.Contains (location);
			if (IsExpired && isInFrame) {
				TitleInfoClick (null);
				return;
			}

			base.MouseUp (theEvent);
		}

		[Action ("TitleInfoClick:")]
		void TitleInfoClick (NSObject sender)
		{
			var controller = (PublicationsWindowController)Utility.GetMainWindowConroller();
			controller.OpenInfoModal ();
		}
			
		//no use
		partial void ExpandButtonClick (NSObject sender)
		{
			var expandButton = (NSButton)sender;
			var tableRowView = (NSTableRowView)expandButton.Superview;
			TocTableView.RowForView (tableRowView);
		}

		#endregion
	}
}

