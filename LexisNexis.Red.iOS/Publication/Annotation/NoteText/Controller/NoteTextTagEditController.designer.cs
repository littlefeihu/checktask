// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace LexisNexis.Red.iOS
{
	[Register ("NoteTextTagEditController")]
	partial class NoteTextTagEditController
	{
		[Outlet]
		UIKit.UICollectionView collectionVC { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (collectionVC != null) {
				collectionVC.Dispose ();
				collectionVC = null;
			}
		}
	}
}
