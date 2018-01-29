using System;

using AppKit;
using Foundation;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	public class ExpandTextView : NSTextView
	{
		#region Constructors

		// Called when created from unmanaged code
		public ExpandTextView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		[Export("initWithFrame:")]
		public ExpandTextView (CGRect frame) : base(frame)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		uint LineNumber (NSTextView textView)
		{
			NSLayoutManager layoutManager = textView.LayoutManager;

			nint lineCharacterRange = layoutManager.NumberOfGlyphs;

			var lineRange = new NSRange (0, lineCharacterRange);
			var charRane = new NSRange (0,1);
			CGRect lineRect = layoutManager.BoundingRectForGlyphRange (lineRange, textView.TextContainer);
			CGRect charRect = layoutManager.BoundingRectForGlyphRange (charRane, textView.TextContainer);

			uint lineNum = (uint)(lineRect.Bottom / charRect.Bottom);
			return lineNum;
		}

		NSRange LineRange (NSTextView textView)
		{
			NSLayoutManager layoutManager = textView.LayoutManager;

			var charRane = new NSRange (0,1);
			CGRect charRect = layoutManager.BoundingRectForGlyphRange (charRane, textView.TextContainer);

			var lRect = new CGRect (charRect.Left, charRect.Size.Height*6, charRect.Size.Width, charRect.Size.Height);

			NSRange lRange = layoutManager.GlyphRangeForBoundingRect (lRect, textView.TextContainer);
			var nOrange = new NSRange (0, lRange.Location - 1);

			return nOrange;
		}

		nfloat HeightWrappedToViewWidth (NSTextView textView, float width)
		{
			NSLayoutManager layoutManager = textView.LayoutManager;
			NSTextContainer container = textView.TextContainer;

			layoutManager.GetGlyphRange (container); //force layout
			nfloat height = layoutManager.GetUsedRectForTextContainer(container).Height;

			return height;
		}

		nfloat HeightWrappedToWidth (float width)
		{
			nfloat height = 0;
			NSTextStorage storage = new NSTextStorage ();
			NSLayoutManager layoutManager = new NSLayoutManager ();
			CGSize size = new CGSize (width, 100);
			NSTextContainer container = new NSTextContainer (size);

			layoutManager.AddTextContainer(container);
			storage.AddLayoutManager(layoutManager);
			layoutManager.GetGlyphRange (container); //force layout
			height = layoutManager.GetUsedRectForTextContainer(container).Height;

			return height;
		}



	}
}

