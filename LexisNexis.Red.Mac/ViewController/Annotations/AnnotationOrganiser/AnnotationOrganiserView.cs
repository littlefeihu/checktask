using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace LexisNexis.Red.Mac
{
	public partial class AnnotationOrganiserView : AppKit.NSView
	{
		#region Constructors

		// Called when created from unmanaged code
		public AnnotationOrganiserView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public AnnotationOrganiserView (NSCoder coder) : base (coder)
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
