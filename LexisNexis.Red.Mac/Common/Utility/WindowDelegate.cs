using System;
using AppKit;
using Foundation;

namespace LexisNexis.Red.Mac
{
	public class WindowDelegate : NSWindowDelegate {
		public WindowDelegate ()
		{
		}

		public override bool WindowShouldClose (NSObject sender)
		{
			NSApplication.SharedApplication.StopModalWithCode(LNRConstants.ID_CLOSE);

			return true;
		}
	}
}

