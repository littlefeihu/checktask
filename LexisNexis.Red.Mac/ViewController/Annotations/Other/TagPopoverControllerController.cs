using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace LexisNexis.Red.Mac
{
	public partial class TagPopoverControllerController : AppKit.NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public TagPopoverControllerController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public TagPopoverControllerController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public TagPopoverControllerController () : base ("TagPopoverController", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new TagPopoverController View {
			get {
				return (TagPopoverController)base.View;
			}
		}
	}
}
