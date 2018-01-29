
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	public partial class PublicationsWindow : AppKit.NSWindow
	{

		#region Constructors

		// Called when created from unmanaged code
		public PublicationsWindow (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public PublicationsWindow (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		public CGPoint ToolbarButtonOrigin
		{
			get
			{
				CGPoint location = new CGPoint (86, 660);
					
				return location;
			}
		}
	
	}
}

