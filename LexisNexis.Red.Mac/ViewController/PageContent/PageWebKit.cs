using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WebKit;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	public partial class PageWebKit : WebView
	{
		#region Constructors

		// Called when created from unmanaged code
		public PageWebKit (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public PageWebKit (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		public override bool MaintainsInactiveSelection {
			get {
				return true;
			}
		}
	}
}
