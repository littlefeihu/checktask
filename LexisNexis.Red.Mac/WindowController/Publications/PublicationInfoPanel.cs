
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace LexisNexis.Red.Mac
{
	public partial class PublicationInfoPanel : AppKit.NSWindow
	{
		#region Constructors

		// Called when created from unmanaged code
		public PublicationInfoPanel (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public PublicationInfoPanel (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		/*
		public override NSObjectPredicate WindowShouldClose
		{
			//NSApplication.SharedApplication.StopModal ();
			//Window.OrderOut (null);

			return true;
		}*/
	}
}

