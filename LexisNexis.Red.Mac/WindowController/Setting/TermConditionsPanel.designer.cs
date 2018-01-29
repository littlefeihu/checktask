// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace LexisNexis.Red.Mac
{
	[Register ("TermConditionsPanelController")]
	partial class TermConditionsPanelController
	{
		[Outlet]
		AppKit.NSTextView ContentTextView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ContentTextView != null) {
				ContentTextView.Dispose ();
				ContentTextView = null;
			}
		}
	}

	[Register ("TermConditionsPanel")]
	partial class TermConditionsPanel
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
