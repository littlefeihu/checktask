using System;
using AppKit;
using Foundation;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	public class ExpandTableView : NSTableView
	{
		#region Constructors
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ExpandTableView (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		[Export("initWithFrame:")]
		public ExpandTableView (CGRect frame) : base(frame)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}
		#endregion

		public override void DrawRow (nint row, CGRect clipRect)
		{
			if (row >= RowCount) {
				return;
			}

			base.DrawRow (row, clipRect);

			NSGraphicsContext.GlobalSaveGraphicsState();
			NSGraphicsContext.CurrentContext.ShouldAntialias = false;

			CGRect rectRow = RectForRow (row);
			CGRect rectColumn = RectForColumn (0);
			CGRect rect = Frame;

			CGPoint start = new CGPoint(rectColumn.Left, rectRow.Top);
			CGPoint end = new CGPoint(rectRow.Right, rectRow.Top);

			var linePath = new NSBezierPath ();
			GridColor.Set();
			linePath.MoveTo (start);
			linePath.LineTo(end);
			linePath.ClosePath ();
			linePath.Stroke ();

			NSGraphicsContext.GlobalRestoreGraphicsState();
			 
		}

		public override void HighlightSelection (CGRect clipRect)
		{
		}

		//		public override void DrawGrid (RectangleF clipRect)
		//		{
		//			base.DrawGrid (clipRect);
		//		}
	}
}

