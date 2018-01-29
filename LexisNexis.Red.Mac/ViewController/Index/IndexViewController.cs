using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using LexisNexis.Red.Mac.Data;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Mac
{
	public partial class IndexViewController : AppKit.NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public IndexViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public IndexViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public IndexViewController () : base ("IndexView", NSBundle.MainBundle)
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
		public new IndexView View {
			get {
				return (IndexView)base.View;
			}
		}

		public int BookID { get; set;}
		public IndexDataManager IndexDataManager { get;set;}

		#endregion

		#region methods
		public void InitializeOutlineView () 
		{
			if (IndexDataManager.IndexNodeList == null || IndexDataManager.IndexNodeList.Count == 0) {
				IndexLabel.StringValue = "Publication Index";
				IndexInfoLabel.StringValue = "There are no index files available for this publication.";
				InfoCustomView.Hidden = false;
				IndexOutlineView.EnclosingScrollView.Hidden = true;
			} else {
				
				IndexOutlineView.EnclosingScrollView.Hidden = false;
				InfoCustomView.Hidden = true;

				//IndexOutlineView.SelectionShouldChange += (t) => true;
				//IndexOutlineView.EnclosingScrollView.BackgroundColor = NSColor.Clear;
				IndexOutlineView.DataSource = new IndexTableViewDataSources (this);
				IndexOutlineView.Delegate = new IndexTableViewDelegate (this);
				IndexOutlineView.EnclosingScrollView.BorderType = NSBorderType.NoBorder;
				IndexOutlineView.GridStyleMask = NSTableViewGridStyle.DashedHorizontalGridLine;

				IndexOutlineView.AllowsColumnSelection = false;
				IndexOutlineView.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.Regular;
				IndexOutlineView.EnclosingScrollView.HasHorizontalScroller = false;
				IndexOutlineView.AllowsColumnResizing = true;
				IndexOutlineView.EnclosingScrollView.ScrollerKnobStyle = NSScrollerKnobStyle.Default;
				IndexOutlineView.EnclosingScrollView.VerticalScroller.ControlSize = NSControlSize.Small;

				IndexOutlineView.FloatsGroupRows = true;

				NSAnimationContext.BeginGrouping();
				NSAnimationContext.CurrentContext.Duration = 0;
				IndexOutlineView.ExpandItem (null, true);
				NSAnimationContext.EndGrouping();
			}
		}

		public void SelectItemByRow(nint row) {
			IndexOutlineView.SelectRow (row, false);
		}

		public void RefreshOutlineViewData()
		{
			IndexOutlineView.ReloadData ();
		}

		public void OpenPublicationIndexAtIndexNode (Index indexNode)
		{
			IndexDataManager.PanelController.OpenPublicationIndexAtIndexNode (indexNode);
		}

		public void RefreshSelectBackColor ()
		{
			nint row = IndexOutlineView.SelectedRow;
			if (row >= 0) {
				var rowView = (IndexTableRowView)IndexOutlineView.GetRowView (row, true);
				rowView.SelBgColor = NSColor.White;
				IndexOutlineView.ReloadData ();
			}
		}

		#endregion
	}
}
