using System;
using AppKit;
using Foundation;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	public class IndexTableRowView : NSTableRowView
	{
		public int Level { get; set; }
		public int RowIndex { get; set;}

		public NSColor SelBgColor{ get; set; }
		#region Constructors

		// Called when created from unmanaged code
		public IndexTableRowView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public IndexTableRowView (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		[Export ("init")]
		public IndexTableRowView ()
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion
		public override NSBackgroundStyle InteriorBackgroundStyle {
			get {
				return NSBackgroundStyle.Light;
			}
		}

		public override void DrawSelection (CGRect dirtyRect)
		{
			// Check the selectionHighlightStyle, in case it was set to None

			if (SelectionHighlightStyle != NSTableViewSelectionHighlightStyle.None) {
				if (SelBgColor == null) {
					SelBgColor = NSColor.LightGray;
				}
					
				SelBgColor.Set ();
				var selectionPath = NSBezierPath.FromRoundedRect (Bounds, 0, 0);
				selectionPath.Fill ();
			}
		}
	}
}

