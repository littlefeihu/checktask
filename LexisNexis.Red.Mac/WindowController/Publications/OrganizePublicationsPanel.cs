
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace LexisNexis.Red.Mac
{
	public partial class OrganizePublicationsPanel : AppKit.NSWindow
	{
		#region Constructors

		// Called when created from unmanaged code
		public OrganizePublicationsPanel (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public OrganizePublicationsPanel (NSCoder coder) : base (coder)
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

