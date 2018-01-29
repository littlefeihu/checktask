
using System;

using Foundation;
using AppKit;
using CoreGraphics;

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

		public override void DrawSelection (CGRect dirtyRect)
		{
			// Check the selectionHighlightStyle, in case it was set to None

			if (Frame.Top < LNRConstants.TOCITEMHEIGHT_MIN && Frame.Bottom <= LNRConstants.TOCITEMHEIGHT_MIN+9) {
				return;
			}
				
			if (SelectionHighlightStyle != NSTableViewSelectionHighlightStyle.None) {
				Utility.BlendNSColor (Level, LNRConstants.TOCLEVEL_MAX).Set ();
				var selectionPath = NSBezierPath.FromRoundedRect (Bounds, 0, 0);
				selectionPath.Fill ();

				var cellView = (NSTableCellView)Subviews [0];
				var textField = cellView.TextField;
				NSAttributedString attributeTitle = Utility.AttributedTitle (textField.StringValue, 
					                                    NSColor.White, textField.Font.FontName,
					                                    (float)textField.Font.PointSize,
					                                    NSTextAlignment.Left);

				textField.AttributedStringValue = attributeTitle;

				//0:color 1:title 2:button 3:boxline
				var button = (NSButton)cellView.Subviews [2];
				if (button.Tag == 3 && !IsSubSelect) {
					button.Cell.Image = Utility.ImageWithFilePath ("/Images/TOC/toc-doc-white.png");
				}	
			}
		}

		public override void DrawBackground (CGRect dirtyRect)
		{
			base.DrawBackground(dirtyRect);

			NSColor textColor;
			if (IsParent || IsSubSelect) {
				textColor = NSColor.White;
			} else {
				textColor = NSColor.Black;
			}

			var cellView = (NSTableCellView)Subviews [0];
			var textField = cellView.TextField;
			NSAttributedString attributeTitle = Utility.AttributedTitle (textField.StringValue, 
				textColor, textField.Font.FontName,
				(float)textField.Font.PointSize,
				NSTextAlignment.Left);

			textField.AttributedStringValue = attributeTitle;

			var button = (NSButton)cellView.Subviews [2];
			if (button.Tag == 3) {
				if (IsSubSelect) {
					button.Cell.Image = Utility.ImageWithFilePath ("/Images/TOC/toc-doc-white.png");
				} else {
					button.Cell.Image = Utility.ImageWithFilePath ("/Images/TOC/toc_doc.png");
				}
			}

			if (IsParent) {
				Utility.BlendNSColor (Level, LNRConstants.TOCLEVEL_MAX).Set ();
			} else if (IsSubSelect){
				Utility.BlendNSColor (Level, LNRConstants.TOCLEVEL_MAX).Set ();
			} else{
				NSColor.White.Set ();
			}

			NSBezierPath selectionPath = NSBezierPath.FromRoundedRect (Bounds, 0, 0);
			selectionPath.Fill ();
		}
	}
}

