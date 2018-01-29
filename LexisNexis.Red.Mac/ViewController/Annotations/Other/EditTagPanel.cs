using System;

using Foundation;
using AppKit;

namespace LexisNexis.Red.Mac
{
	public partial class EditTagPanel : NSWindow
	{
		public EditTagPanel (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public EditTagPanel (NSCoder coder) : base (coder)
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
		}
	}
}
