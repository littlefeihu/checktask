using System;

using Foundation;
using AppKit;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	public class ExpandTextField : NSTextField
	{
		#region Constructors

		// Called when created from unmanaged code
		public ExpandTextField (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		[Export("initWithFrame:")]
		public ExpandTextField (CGRect frame) : base(frame)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		nfloat HeightWrappedToWidth (float width)
		{
			NSTextField textField = new NSTextField(new CGRect(0, 0, width, 1000));
			textField.Font = NSFont.SystemFontOfSize(14);
			textField.StringValue = StringValue;

			CGSize size = textField.Cell.CellSizeForBounds(textField.Frame);

			return size.Height;
		}

	}
}

