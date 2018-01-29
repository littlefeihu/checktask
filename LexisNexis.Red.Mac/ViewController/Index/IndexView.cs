using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace LexisNexis.Red.Mac
{
	public partial class IndexView : AppKit.NSView
	{
		#region Constructors

		// Called when created from unmanaged code
		public IndexView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public IndexView (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion
	}
}
