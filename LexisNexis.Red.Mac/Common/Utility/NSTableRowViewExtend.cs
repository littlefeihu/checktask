
using System;
using System.Drawing;

using MonoMac.Foundation;
using MonoMac.AppKit;

namespace LexisNexis.Red.Mac
{
	public partial class NSTableRowViewExtend : NSTableRowView
	{
		#region

		public int Level { get; set; }
		public int RowIndex { get; set;}
		public bool IsParent { get; set;}
		public bool IsSubSelect { get; set;}
		#endregion

		#region Constructors

		// Called when created from unmanaged code
		public NSTableRowViewExtend (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public NSTableRowViewExtend (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		[Export ("init")]
		public NSTableRowViewExtend ()
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		public override void DrawSelection (RectangleF dirtyRect)
		{
			// Check the selectionHighlightStyle, in case it was set to None

			if (Frame.Top <= LNRConstants.TOCITEMHEIGHT_MIN) {
				return;
			}

			if (SelectionHighlightStyle != NSTableViewSelectionHighlightStyle.None) {

				Utility.BlendNSColor (Level, LNRConstants.TOCLEVEL_MAX).Set();

				NSBezierPath selectionPath = NSBezierPath.FromRoundedRect (Bounds, 0, 0);
				selectionPath.Fill ();
			}
		}
			
		public override void DrawBackground (RectangleF dirtyRect)
		{
			if (IsParent) {
				Utility.BlendNSColor (Level, LNRConstants.TOCLEVEL_MAX).Set ();
			} else if (IsSubSelect){
				//IsSubSelect = false;
				Utility.BlendNSColor (Level, LNRConstants.TOCLEVEL_MAX).Set ();
			}
			else {
				NSColor.White.Set();
			}

			NSBezierPath selectionPath = NSBezierPath.FromRoundedRect (Bounds, 0, 0);
			selectionPath.Fill ();

		}
	}
}

