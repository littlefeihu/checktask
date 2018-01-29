using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using LexisNexis.Red.Mac.Data;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Business;

namespace LexisNexis.Red.Mac
{
	public partial class HistoryPopoverController : AppKit.NSViewController
	{
		#region properties
		//NSPopover parentPopover;
		public List<Publication> offlinePublicationList;
		#endregion

		#region Constructors

		// Called when created from unmanaged code
		public HistoryPopoverController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public HistoryPopoverController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public HistoryPopoverController (NSPopover popover) : base ("HistoryPopover", NSBundle.MainBundle)
		{
			//parentPopover = popover;
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
			offlinePublicationList = PublicationsDataManager.SharedInstance.GetLocalDBPublicationsList();
		}

		#endregion

		//strongly typed view accessor
		public new HistoryPopover View {
			get {
				return (HistoryPopover)base.View;
			}
		}

		#region methods
		public override void AwakeFromNib ()
		{
			PostInitialization ();
		}

		void PostInitialization ()
		{
			//HistoryLabel.StringValue = "Recent History";
			HistoryTableView.BackgroundColor = NSColor.White;
			HistoryTableView.DataSource = new PopHistoryTableDataSource(offlinePublicationList);
			HistoryTableView.Delegate = new PopHistoryTableDelegate(offlinePublicationList);
			HistoryTableView.GridStyleMask = NSTableViewGridStyle.None;
			HistoryTableView.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.Regular;

		}
		#endregion
	}
}
