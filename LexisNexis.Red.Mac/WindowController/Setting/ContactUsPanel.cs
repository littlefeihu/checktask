using System;

using Foundation;
using AppKit;

namespace LexisNexis.Red.Mac
{
	public partial class ContactUsPanel : NSWindow
	{
		public ContactUsPanel (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public ContactUsPanel (NSCoder coder) : base (coder)
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
		}
	}
}
