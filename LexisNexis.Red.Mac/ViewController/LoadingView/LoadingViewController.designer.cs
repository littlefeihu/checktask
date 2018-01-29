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
	[Register ("LoadingViewController")]
	partial class LoadingViewController
	{
		[Outlet]
		AppKit.NSTextField LoadInfoTF { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (LoadInfoTF != null) {
				LoadInfoTF.Dispose ();
				LoadInfoTF = null;
			}
		}
	}
}
