
using System;

using Foundation;
using AppKit;

using CoreGraphics;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Mac
{
	public partial class BookCoverView : NSView
	{
		#region Constructors

		// Called when created from unmanaged code
		public BookCoverView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public BookCoverView (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		[Export("initWithFrame:")]
		public BookCoverView (CGRect frame) : base(frame)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		public string ColorPrimary {get; set;}
		public string FontColor {get; set;}
		public string ColorSecondary {get; set;}

		public string Title { get; set;}
		public int DaysRemaining { get; set;}
		public bool IsLoan { get; set;}
		public int Index { get; set;}
		public bool IsFTC { get; set;}
		public void InitializeValue (int curIndex)
		{
			Index = curIndex;

			AddTitleFrame ();
			AddLoanFrame ();
	
			AddFTCImageView ();
		}

		void AddTitleFrame ()
		{
			string titleName = Title;

			CGRect titleFrame;

			if (IsFTC) {
				int location = titleName.IndexOf ("+ Cases");
				if (location > 0) {
					titleName = titleName.Substring (0, location);
				}else {
					location = titleName.IndexOf ("+Case");
					if (location > 0) {
						titleName = titleName.Substring (0, location);
					}
				}
				titleFrame = TitleCaseFrameWithTitle(titleName);
			} else {
				titleFrame = TitleFrameWithTitle(titleName);
			}

			var titleTF = new NSTextField (titleFrame);
			titleTF.Cell.Bordered = false;
			titleTF.Cell.DrawsBackground = false;
			titleTF.Cell.Editable = false;
			titleTF.Cell.LineBreakMode = NSLineBreakMode.ByWordWrapping;
			titleTF.Cell.TruncatesLastVisibleLine = true;
			titleTF.ToolTip = Title;

			NSAttributedString attributeTitle = Utility.AttributedTitle(titleName, 
				Utility.ColorWithHexColorValue (FontColor,1.0f), "Garamond",14.5f,NSTextAlignment.Center);
			titleTF.AttributedStringValue = attributeTitle;
			//string name = titleTF.Font.FontName;
			AddSubview (titleTF);

			if (IsFTC) {
				CGRect caseFrame = new CGRect (titleFrame.Left,titleFrame.Top-2-PUBLICATION_TITLE_LINTHEIGHT,titleFrame.Width,PUBLICATION_TITLE_LINTHEIGHT);
				var caseTF = new NSTextField (caseFrame);
				caseTF.Cell.Bordered = false;
				caseTF.Cell.DrawsBackground = false;
				caseTF.Cell.Editable = false;
				caseTF.Cell.Alignment = NSTextAlignment.Justified;
				caseTF.Cell.LineBreakMode = NSLineBreakMode.TruncatingTail;
				caseTF.Cell.TruncatesLastVisibleLine = true;

				NSAttributedString caseTitle = Utility.AttributedTitle("+ Cases", 
					Utility.ColorWithHexColorValue (FontColor,1.0f), "Garamond",14.5f,NSTextAlignment.Center);
				caseTF.AttributedStringValue = caseTitle;
				AddSubview (caseTF);
			}
		}

		void AddLoanFrame ()
		{
			if (!IsLoan) {
				return;
			}

			var loanTF = new NSTextField (loanFrame);
			loanTF.Cell.Bordered = false;
			loanTF.Cell.DrawsBackground = false;
			loanTF.Cell.Editable = false;
			NSAttributedString attributeTitle = Utility.AttributedTitle("LOAN", NSColor.Grid, "Garamond",14.5f,NSTextAlignment.Center);
			loanTF.AttributedStringValue = attributeTitle;
			AddSubview (loanTF);

			string loanInfoStr = DaysRemaining + " days Remaining";
			var loanDateTF = new NSTextField (loanDateFrame);
			loanDateTF.Cell.Bordered = false;
			loanDateTF.Cell.DrawsBackground = false;
			loanDateTF.Cell.Editable = false;

			if (DaysRemaining > 0) {
				attributeTitle = Utility.AttributedTitle(loanInfoStr, NSColor.Grid, "Garamond",14.5f,NSTextAlignment.Center);
				loanDateTF.AttributedStringValue = attributeTitle;
			} else {
				attributeTitle = Utility.AttributedTitle ("Due to expired", NSColor.Grid, "Garamond", 14.5f, NSTextAlignment.Center);
				loanDateTF.AttributedStringValue = attributeTitle;
			}
			AddSubview (loanDateTF);
		}

		void AddFTCImageView ()
		{
			if (IsFTC) {
				nfloat ratio = (nfloat)230 / 350;
				var frame = new CGRect (10, PUBLICATION_COVER_HEIGHT - 50*ratio, 30*ratio, 50*ratio);
				var imageView = new NSImageView (frame);
				imageView.Image = Utility.ImageWithFilePath ("/Images/Publication/FTC.png");
				AddSubview (imageView);
			}
		}

		public override void DrawRect (CGRect dirtyRect)
		{
			base.DrawRect (dirtyRect);

			NSGraphicsContext.GlobalSaveGraphicsState();

			//backgound frame
			DrawCoverRect();

			//title frame
			DrawTitleRect();

			//
			//DrawUpdateView();

			//bottom line
			DrawBottomLine();

			NSGraphicsContext.GlobalRestoreGraphicsState();

		}

		const float PUBLICATION_COVER_ORGY = 0.0f;
		const float PUBLICATION_COVER_WIDTH = 170.0f;
		const float PUBLICATION_COVER_HEIGHT = 230.0f;

		const float PUBLICATION_TITLE_ORGY = 110.5f;
		const float PUBLICATION_LINT_WIDTH = 145.0f;
		const float PUBLICATION_LINT_HEIGHT = 93.5f;

		const float PUBLICATION_TITLE_LINTHEIGHT = 20.0f;

		const float PUBLICATION_LOAN_ORGY = 42.5f;
		const float PUBLICATION_LOANDAY_ORGY = 25.5f;
		const float PUBLICATION_COVERLINE_SPACE = 12.5f;

		void DrawCoverRect ()
		{
			Utility.ColorWithHexColorValue (ColorPrimary,1.0f).Set ();
			NSGraphics.RectFill (coverFrame);
		}

		void DrawTitleRect ()
		{
			Utility.ColorWithHexColorValue (ColorSecondary,1.0f).Set ();
			NSGraphics.RectFill (titleBorderFrame);

			var oPath = NSBezierPath.FromRect (wideBorderLineFrame);
			Utility.ColorWithHexColorValue (FontColor,1.0f).Set();
			oPath.LineWidth = 3;
			oPath.Stroke ();

			var iPath = NSBezierPath.FromRect (thinBorderLineFrame);
			Utility.ColorWithHexColorValue (ColorSecondary,1.0f).Set();
			iPath.LineWidth = 1;
			iPath.Stroke ();
		}

		void DrawBottomLine ()
		{
			var linePath = new NSBezierPath ();

			Utility.ColorWithHexColorValue (FontColor,1.0f).SetStroke ();

			linePath.LineWidth = 2;
			linePath.MoveTo (bottomLineTS);
			linePath.LineTo(bottomLineTE);
			linePath.ClosePath ();

			linePath.Stroke ();

			//var lineTPath = new NSBezierPath ();
			linePath.MoveTo (bottomLineBS);
			linePath.LineTo(bottomLineBE);
			linePath.ClosePath ();
			linePath.Stroke ();
		}


		//
		static CGRect coverFrame = new CGRect (0,
			PUBLICATION_COVER_ORGY,
			PUBLICATION_COVER_WIDTH,
			PUBLICATION_COVER_HEIGHT);

		//
		static CGRect wideBorderLineFrame = new CGRect (PUBLICATION_COVERLINE_SPACE,
			PUBLICATION_TITLE_ORGY,
			PUBLICATION_LINT_WIDTH,
			PUBLICATION_LINT_HEIGHT);


		static CGRect thinBorderLineFrame = new CGRect (PUBLICATION_COVERLINE_SPACE,
			PUBLICATION_TITLE_ORGY,
			PUBLICATION_LINT_WIDTH,
			PUBLICATION_LINT_HEIGHT);

		//
		static CGRect titleBorderFrame = new CGRect (PUBLICATION_COVERLINE_SPACE+2,
			PUBLICATION_TITLE_ORGY+2,
			PUBLICATION_LINT_WIDTH-4,
			PUBLICATION_LINT_HEIGHT-4);

		static CGRect TitleFrameWithTitle (string title)
		{
			NSFont newFont = NSFont.FromFontName ("Garamond", 14.5f);
			nfloat textHeight = HeightWrappedToWidth (title, newFont);
			if (textHeight > 80) {
				textHeight = 80;
			}

			return new CGRect (PUBLICATION_COVERLINE_SPACE + 6,
				PUBLICATION_TITLE_ORGY + (PUBLICATION_LINT_HEIGHT - textHeight)/2,
				PUBLICATION_LINT_WIDTH - 6 * 2,
				textHeight);

		}

		static CGRect TitleCaseFrameWithTitle (string title)
		{
			NSFont newFont = NSFont.FromFontName ("Garamond", 14.5f);
			nfloat textHeight = HeightWrappedToWidth (title, newFont);
			if (textHeight > 60) {
				textHeight = 60;
			}
			
			return new CGRect (PUBLICATION_COVERLINE_SPACE + 6,
				PUBLICATION_TITLE_ORGY + (PUBLICATION_LINT_HEIGHT/2 - textHeight/2)+PUBLICATION_TITLE_LINTHEIGHT/2,
				PUBLICATION_LINT_WIDTH - 6 * 2,
				textHeight);

		}

		static nfloat HeightWrappedToWidth (string title, NSFont font)
		{
			NSAttributedString textAttrStr = new NSAttributedString (title, font);
			CGSize maxSize = new CGSize (PUBLICATION_LINT_WIDTH - 6 * 2, 1000);
			CGRect boundRect = textAttrStr.BoundingRectWithSize (maxSize,
				NSStringDrawingOptions.TruncatesLastVisibleLine | 
				NSStringDrawingOptions.UsesLineFragmentOrigin | 
				NSStringDrawingOptions.UsesFontLeading);

			//multiple of 17
			nfloat stringHeight = boundRect.Height;

			return stringHeight;
		}

		//
		static CGRect loanFrame = new CGRect (PUBLICATION_COVERLINE_SPACE,
			PUBLICATION_LOAN_ORGY,
			PUBLICATION_LINT_WIDTH,
			PUBLICATION_TITLE_LINTHEIGHT);

		static CGRect loanDateFrame = new CGRect (PUBLICATION_COVERLINE_SPACE,
			PUBLICATION_LOANDAY_ORGY,
			PUBLICATION_LINT_WIDTH,
			PUBLICATION_TITLE_LINTHEIGHT);
		
		//
		static CGPoint bottomLineTS = new CGPoint (PUBLICATION_COVERLINE_SPACE, 
			PUBLICATION_COVER_ORGY+3+PUBLICATION_COVERLINE_SPACE);

		static CGPoint bottomLineTE = new CGPoint (PUBLICATION_COVER_WIDTH-PUBLICATION_COVERLINE_SPACE, 
			PUBLICATION_COVER_ORGY+3+PUBLICATION_COVERLINE_SPACE);

		static CGPoint bottomLineBS = new CGPoint (PUBLICATION_COVERLINE_SPACE, 
			PUBLICATION_COVER_ORGY+PUBLICATION_COVERLINE_SPACE);

		static CGPoint bottomLineBE = new CGPoint (PUBLICATION_COVER_WIDTH-PUBLICATION_COVERLINE_SPACE, 
			PUBLICATION_COVER_ORGY+PUBLICATION_COVERLINE_SPACE);
	}
}

